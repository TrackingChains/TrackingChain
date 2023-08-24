using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                string txHash = "";
                string writerEndpointAddress = "";
                try
                {
                    writerEndpointAddress = account.GetFirstRandomWriterAddress;
                    txHash = await blockChainService.InsertTrackingAsync(
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

                    pool.UnlockFromError(reTryAfterSeconds);

                    if (pool.ErrorTimes <= errorAfterReTry)
                    {
                        var report = new Core.Domain.Entities.Report(
                        TrackinChainExceptionExtensions.GetAllExceptionMessages(ex),
                        0,
                        false,
                        ReportType.TxGenerationFailed,
                        pool.TrackingId);
                        applicationDbContext.Add(report);
                    }
                    else
                        await SetTransactionGenerationCompletedInErrorAsync(errorAfterReTry, pool);

                    await applicationDbContext.SaveChangesAsync();

                    logger.TransactionOnChain(pool.TrackingId, "TxFailed", pool.SmartContractAddress);
                    return pool.TrackingId;
                }

                // Tx Generated.
                var txPending = transactionGeneratorService.AddTransactionPendingFromPool(pool, txHash);
                await transactionGeneratorService.SetToPendingAsync(txPending.TrackingId, txHash, account.ChainWriterAddress);

                await applicationDbContext.SaveChangesAsync();

                logger.TransactionOnChain(pool.TrackingId, txPending.TxHash, pool.SmartContractAddress);
                return pool.TrackingId;
            }

            return Guid.Empty;
        }

        // Helpers.
        private async Task SetTransactionGenerationCompletedInErrorAsync(int errorAfterReTry, TransactionPool pool)
        {
            var report = new Core.Domain.Entities.Report(
                                    $"Exceed Retry Limit\tErrorTimes: {pool.ErrorTimes}\tErrorAfterReTry: {errorAfterReTry}",
                                    0,
                                    false,
                                    ReportType.TxGenerationInError,
                                    pool.TrackingId);
            applicationDbContext.Add(report);

            pool.SetStatusError();

            await transactionGeneratorService.SetToRegistryErrorAsync(pool.TrackingId);

            logger.TransactionGenerationCompletedInError(pool.TrackingId);
        }
    }
}
