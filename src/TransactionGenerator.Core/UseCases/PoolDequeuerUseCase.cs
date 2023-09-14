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
            int errorAfterReTry)
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
                if (pool.ErrorTimes > errorAfterReTry)
                {
                    await SetTransactionGenerationCompletedInErrorAsync(errorAfterReTry, pool);

                    await applicationDbContext.SaveChangesAsync();

                    logger.TransactionOnChain(
                        pool.TrackingId, 
                        $"Exceed Retry Limit\tErrorTimes: {pool.ErrorTimes}\tErrorAfterReTry: {errorAfterReTry}", 
                        pool.SmartContractAddress);
                    return pool.TrackingId;
                }

                var blockChainService = blockchainServices.First(x => x.ProviderType == pool.ChainType);

                TransactionDetail? transactionDetail = null;
                ContractExtraInfo contractExtraInfo;
                string writerEndpointAddress = "";
                try
                {
                    writerEndpointAddress = account.GetFirstRandomWriterAddress;
                    contractExtraInfo = ContractExtraInfo.FromJson(pool.SmartContractExtraInfo);
                    transactionDetail = await blockChainService.InsertTrackingAsync(
                    pool.Code,
                    pool.DataValue,
                    account.PrivateKey,
                    pool.ChainNumberId,
                    writerEndpointAddress,
                    pool.SmartContractAddress,
                    contractExtraInfo,
                    CancellationToken.None);

                    if (transactionDetail is null)
                        throw new InvalidOperationException("TransactionDetail is NULL");
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.TrasactionGenerationInError(pool.TrackingId, writerEndpointAddress, ex);

                    pool.UnlockFromError(TransactionErrorReason.InsertTransactionExpection, reTryAfterSeconds);

                    if (pool.ErrorTimes <= errorAfterReTry)
                    {
                        var reportItem = new Core.Domain.Entities.ReportItem(
                        TrackinChainExceptionExtensions.GetAllExceptionMessages(ex),
                        0,
                        false,
                        ReportItemType.TxGenerationFailed,
                        pool.TrackingId);
                        applicationDbContext.Add(reportItem);
                    }
                    else
                        await SetTransactionGenerationCompletedInErrorAsync(errorAfterReTry, pool);

                    await applicationDbContext.SaveChangesAsync();

                    logger.TransactionOnChain(pool.TrackingId, "TxFailed", pool.SmartContractAddress);
                    return pool.TrackingId;
                }

                // Tx Generated.
                var txPending = transactionGeneratorService.AddTransactionPendingFromPool(pool, transactionDetail.TransactionHash);
                await transactionGeneratorService.SetToPendingAsync(txPending.TrackingId, transactionDetail.TransactionHash, account.ChainWriterAddress);
                if (contractExtraInfo.WaitingForResult)
                    await TransactionExecutedSuccessAsync(pool, txPending, transactionDetail);

                await applicationDbContext.SaveChangesAsync();

                logger.TransactionOnChain(pool.TrackingId, txPending.TxHash, pool.SmartContractAddress);
                return pool.TrackingId;
            }

            return Guid.Empty;
        }

        // Helpers.
        private async Task SetTransactionGenerationCompletedInErrorAsync(int errorAfterReTry, TransactionPool pool)
        {
            var reportItem = new Core.Domain.Entities.ReportItem(
                                    $"Exceed Retry Limit\tErrorTimes: {pool.ErrorTimes}\tErrorAfterReTry: {errorAfterReTry}",
                                    0,
                                    false,
                                    ReportItemType.TxGenerationInError,
                                    pool.TrackingId);
            applicationDbContext.Add(reportItem);

            pool.SetStatusError();

            await transactionGeneratorService.SetToRegistryErrorAsync(
                pool.TrackingId, 
                new TransactionDetail(TransactionErrorReason.UnableToSendTransactionOnChain));

            logger.TransactionGenerationCompletedInError(pool.TrackingId);
        }

        private async Task TransactionExecutedSuccessAsync(
            TransactionPool pool,
            TransactionPending pending,
            TransactionDetail transactionDetail)
        {
            if (transactionDetail.Successful.HasValue &&
                !transactionDetail.Successful.Value)
            {
                if (transactionDetail.TransactionErrorReason is null)
                {
                    var ex = new InvalidOperationException("TransactionErrorReason is mandatory");
                    ex.Data.Add("TrackingId", pending.TrackingId);
                    throw ex;
                }

                await transactionGeneratorService.SetToRegistryErrorAsync(
                    pending.TrackingId,
                    transactionDetail);

                pending.SetStatusError();
            }
            else
            {
                await transactionGeneratorService.SetToRegistryCompletedAsync(
                    pending.TrackingId,
                    transactionDetail);
                await transactionGeneratorService.SetTransactionTriageCompletedAsync(pending.TrackingId);
                pool.SetCompleted();
                pending.SetCompleted();
            }
        }
    }
}
