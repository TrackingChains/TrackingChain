using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.Core.Extensions;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionGeneratorCore.Services;

namespace TrackingChain.TransactionGeneratorCore.UseCases
{
    public class PoolDequeuerUseCase : IPoolDequeuerUseCase
    {
        // Fields.
        private readonly IAccountService accountService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IEnumerable<IBlockchainService> blockchainServices;
        private readonly ILogger<PoolDequeuerUseCase> logger;
        private readonly ITransactionGeneratorService transactionGeneratorService;

        // Constructors.
        public PoolDequeuerUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEnumerable<IBlockchainService> blockchainServices,
            ILogger<PoolDequeuerUseCase> logger,
            ITransactionGeneratorService transactionGeneratorService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.blockchainServices = blockchainServices;
            this.logger = logger;
            this.transactionGeneratorService = transactionGeneratorService;
        }

        // Methods.
        public async Task<Guid> DequeueTransactionAsync(
            int max,
            Guid accountId,
            int reTryAfterSeconds,
            int maxErrorTime)
        {
            var account = await accountService.GetAccountAsync(accountId);
            var pools = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(max, accountId);

            foreach (var pool in pools)
            {
                try
                {
                    pool.SetLocked(accountId);
                    await applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    applicationDbContext.Entry(pool).State = EntityState.Unchanged;
                    continue;
                }
                if (pool.ErrorTimes > maxErrorTime)
                {
                    await SetTransactionGenerationCompletedInErrorAsync(maxErrorTime, pool);

                    await applicationDbContext.SaveChangesAsync();

                    logger.TransactionOnChain(
                        pool.TrackingId, 
                        $"Exceed Retry Limit\tErrorTimes: {pool.ErrorTimes}\tMaxErrorTime: {maxErrorTime}", 
                        pool.SmartContractAddress);
                    return pool.TrackingId;
                }

                var blockChainService = blockchainServices.First(x => x.ProviderType == pool.ChainType);

                TransactionDetail? transactionDetail = null;
                string writerEndpointAddress = "";
                string? errorException = null;
                try
                {
                    writerEndpointAddress = account.GetFirstRandomWriterAddress;
                    transactionDetail = await blockChainService.InsertTrackingAsync(
                    pool.Code,
                    pool.DataValue,
                    account.PrivateKey,
                    pool.ChainNumberId,
                    writerEndpointAddress,
                    pool.SmartContractAddress,
                    ContractExtraInfo.FromJson(pool.SmartContractExtraInfo),
                    CancellationToken.None);
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.TrasactionGenerationInError(pool.TrackingId, writerEndpointAddress, ex);
                    errorException = TrackinChainExceptionExtensions.GetAllExceptionMessages(ex);
                }
                transactionDetail ??= new TransactionDetail(TransactionErrorReason.UnableToSendTransactionOnChain);

                if (transactionDetail.Status == TransactionDetailStatus.Failed)
                {
                    pool.UnlockFromError(transactionDetail.TransactionErrorReason ?? TransactionErrorReason.UnableToSendTransactionOnChain, reTryAfterSeconds);
                    applicationDbContext.Update(pool);

                    if (pool.ErrorTimes <= maxErrorTime &&
                        transactionDetail.TransactionErrorReason != TransactionErrorReason.TransactionFinalizedInError)
                    {
                        var reportItem = new Core.Domain.Entities.ReportItem(
                        errorException ?? transactionDetail.TransactionErrorReason.ToString() ?? "Unknow error",
                        0,
                        false,
                        ReportItemType.TxGenerationFailed,
                        pool.TrackingId);

                        applicationDbContext.Add(reportItem);
                    }
                    else
                        await SetTransactionGenerationCompletedInErrorAsync(maxErrorTime, pool);

                    await applicationDbContext.SaveChangesAsync();

                    logger.TransactionOnChain(pool.TrackingId, "TxFailed", pool.SmartContractAddress);

                    return pool.TrackingId;
                }
                else
                {
                    pool.SetCompleted();
                    var txPending = transactionGeneratorService.AddTransactionPendingFromPool(pool, transactionDetail.TransactionHash);
                    if (transactionDetail.WatchOnlyTx)
                    {
                        await transactionGeneratorService.SetToRegistryCompletedAsync(
                            txPending.TrackingId,
                            transactionDetail);

                        await transactionGeneratorService.SetTransactionTriageCompletedAsync(pool.TrackingId);

                        txPending.SetExcluded();
                    }
                    else
                        await transactionGeneratorService.SetToPendingAsync(pool.TrackingId, transactionDetail.TransactionHash, account.ChainWriterAddress);

                    await applicationDbContext.SaveChangesAsync();

                    logger.TransactionOnChain(pool.TrackingId, transactionDetail.TransactionHash, pool.SmartContractAddress);
                    return pool.TrackingId;
                }
            }

            return Guid.Empty;
        }

        // Helpers.
        private async Task SetTransactionGenerationCompletedInErrorAsync(int maxErrorTime, TransactionPool pool)
        {
            var reportItem = new Core.Domain.Entities.ReportItem(
                                    $"Exceed Retry Limit\tErrorTimes: {pool.ErrorTimes}\tMaxErrorTime: {maxErrorTime}",
                                    0,
                                    false,
                                    ReportItemType.TxGenerationInError,
                                    pool.TrackingId);
            applicationDbContext.Add(reportItem);

            pool.SetStatusError();
            applicationDbContext.Update(pool);

            await transactionGeneratorService.SetToRegistryErrorAsync(
                pool.TrackingId, 
                new TransactionDetail(TransactionErrorReason.UnableToSendTransactionOnChain));

            logger.TransactionGenerationCompletedInError(pool.TrackingId);
        }
    }
}
