using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionWatcherCore.Services;

namespace TrackingChain.TransactionWatcherCore.UseCases
{
    public class PendingTransactionWatcherUseCase : IPendingTransactionWatcherUseCase
    {
        // Fields.
        private readonly IAccountService accountService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IEnumerable<IBlockchainService> blockchainServices;
        private readonly ILogger<PendingTransactionWatcherUseCase> logger;
        private readonly ITransactionWatcherService transactionWatcherService;

        // Constructors.
        public PendingTransactionWatcherUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEnumerable<IBlockchainService> blockchainServices,
            ILogger<PendingTransactionWatcherUseCase> logger,
            ITransactionWatcherService transactionWatcherService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.blockchainServices = blockchainServices;
            this.logger = logger;
            this.transactionWatcherService = transactionWatcherService;
        }

        // Methods.
        public async Task<bool> CheckTransactionStatusAsync(
            int max,
            Guid accountId,
            int reTryAfterSeconds,
            int saveAsErrorAfterSeconds)
        {
            var account = await accountService.GetAccountAsync(accountId);
            var pendings = await transactionWatcherService.GetTransactionToCheckAsync(max, accountId);

            foreach (var pending in pendings)
            {
                try
                {
                    pending.SetLocked(account.Id);
                    await applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    applicationDbContext.Entry(pending).State = EntityState.Unchanged;
                    continue;
                }

                var blockChainService = blockchainServices.First(x => x.ProviderType == pending.ChainType);
                var (apiUrl, apiKey) = account.GetFirstRandomWatcherAddress;

                TransactionDetail? transactionDetail;
                if (!string.IsNullOrWhiteSpace(apiUrl))
                {
                    try
                    {
                        transactionDetail = await blockChainService.GetTrasactionReceiptAsync(pending.TxHash, apiUrl, apiKey);
                    }
#pragma warning disable CA1031 // We need fot catch all problems.
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    { //TODO after some time is null so going in error and need manual check  (MileStone2)
                        logger.GetTrasactionReceiptInError(pending.TrackingId, pending.TxHash, apiUrl, ex);
                        if (pending.ReceivedDate.AddSeconds(saveAsErrorAfterSeconds) < DateTime.UtcNow)
                            transactionDetail = new TransactionDetail(TransactionErrorReason.GetTrasactionReceiptExpection);
                        else
                        {
                            pending.Unlock(reTryAfterSeconds);
                            await applicationDbContext.SaveChangesAsync();
                            break;
                        }
                    }
                    if (transactionDetail is null)
                    {
                        if (pending.ReceivedDate.AddSeconds(saveAsErrorAfterSeconds) < DateTime.UtcNow)
                            transactionDetail = new TransactionDetail(TransactionErrorReason.TransactionNotFound);
                        else
                        {
                            pending.Unlock(reTryAfterSeconds);
                            await applicationDbContext.SaveChangesAsync();
                            break;
                        }
                    }
                }
                else
                    transactionDetail = new TransactionDetail(true);

                await TransactionExecutedAsync(pending, transactionDetail);
                await applicationDbContext.SaveChangesAsync();
            }

            return pendings.Any();
        }

        // Helpers.
        private async Task TransactionExecutedAsync(
            TransactionPending pending, 
            TransactionDetail transactionDetail)
        {
            if (transactionDetail.Successful.HasValue &&
                transactionDetail.Successful.Value)
            {
                pending.SetCompleted();
                await transactionWatcherService.SetTransactionPoolCompletedAsync(pending.TrackingId);
                await transactionWatcherService.SetTransactionTriageCompletedAsync(pending.TrackingId);
            }
            
            await transactionWatcherService.SetToRegistryAsync(
                pending.TrackingId,
                transactionDetail);

            logger.TransactionCompleted(pending.TrackingId, transactionDetail.Successful);
        }
    }
}
