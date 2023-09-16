using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.Interfaces;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.Core.Extensions;
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
        public async Task<Guid> CheckTransactionStatusAsync(
            int max,
            Guid accountId,
            int reTryAfterSeconds,
            int errorAfterReTry)
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

                // Check for error limit.
                if (pending.ErrorTimes > errorAfterReTry)
                {
                    AddReportEntity($"Exceed Retry Limit\tErrorTimes: {pending.ErrorTimes}\tErrorAfterReTry: {errorAfterReTry}",
                        pending,
                        ReportItemType.TxWatchingInError);
                    await TransactionExecutedErrorAsync(
                        pending,
                        new TransactionDetail(pending.LastUnlockedError ?? TransactionErrorReason.UnableToWatchTransactionOnChain));

                    await applicationDbContext.SaveChangesAsync();
                    return pending.TrackingId;
                }

                // Execute Watcher. 
                var blockChainService = blockchainServices.First(x => x.ProviderType == pending.ChainType);
                var (apiUrl, apiKey) = account.GetFirstRandomWatcherAddress;
                TransactionDetail? transactionDetail = null;
                var isInError = false;
                if (!string.IsNullOrWhiteSpace(apiUrl))
                {
                    try
                    {
                        transactionDetail = await blockChainService.GetTrasactionReceiptAsync(pending.TxHash, apiUrl, apiKey);
                    }
#pragma warning disable CA1031 // We need fot catch all problems.
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        logger.GetTrasactionReceiptInError(pending.TrackingId, pending.TxHash, apiUrl, ex);
                        isInError = true;

                        if (pending.ErrorTimes <= errorAfterReTry)
                        {
                            AddReportEntity(TrackinChainExceptionExtensions.GetAllExceptionMessages(ex),
                                pending,
                                ReportItemType.TxWatchingFailed);
                            pending.UnlockFromError(TransactionErrorReason.GetTrasactionReceiptExpection, reTryAfterSeconds);
                        }
                    }

                    //transactionDetail null equal to error.
                    if (!isInError &&
                        transactionDetail is null)
                    {
                        isInError = true;

                        if (pending.ErrorTimes <= errorAfterReTry)
                        {
                            AddReportEntity($"TransactionDetail NULL in ErrorTimes: {pending.ErrorTimes}\tErrorAfterReTry: {errorAfterReTry}",
                                    pending,
                                    ReportItemType.TxWatchingFailed);
                            pending.UnlockFromError(TransactionErrorReason.TransactionNotFound, reTryAfterSeconds);
                        }
                    }
                }
                else
                    transactionDetail = new TransactionDetail(true); //empty url means no watcher avaiable

                // Transaction Success.
                if (!isInError &&
                    transactionDetail!.TransactionErrorReason != TransactionErrorReason.TransactionFinalizedInError)
                    await TransactionExecutedSuccessAsync(pending, transactionDetail); 
                else if (transactionDetail?.TransactionErrorReason == TransactionErrorReason.TransactionFinalizedInError ||
                         pending.ErrorTimes > errorAfterReTry)
                {
                    AddReportEntity($"Exceed Retry Limit\tErrorTimes: {pending.ErrorTimes}\tErrorAfterReTry: {errorAfterReTry}",
                        pending,
                        ReportItemType.TxWatchingInError);
                    await TransactionExecutedErrorAsync(
                        pending,
                        transactionDetail ?? new TransactionDetail(pending.LastUnlockedError ?? TransactionErrorReason.UnableToWatchTransactionOnChain));

                    await applicationDbContext.SaveChangesAsync();
                    return pending.TrackingId;
                }

                await applicationDbContext.SaveChangesAsync();

                return pending.TrackingId;
            }

            return Guid.Empty;
        }

        // Helpers.
        private void AddReportEntity(
            string description, 
            TransactionPending pending,
            ReportItemType reportType)
        {
            var reportItem = new Core.Domain.Entities.ReportItem(
                description,
                0,
                false,
                reportType,
                pending.TrackingId);
            applicationDbContext.Add(reportItem);
        }

        private async Task TransactionExecutedErrorAsync(
            TransactionPending pending,
            TransactionDetail transactionDetail)
        {
            if (!transactionDetail.Successful.HasValue ||
                transactionDetail.Successful.Value)
            {
                var ex = new InvalidOperationException("TransactionExecutedError must be call only with failed TransactionDetail");
                ex.Data.Add("TrackingId", pending.TrackingId);
                throw ex;
            }
            if (transactionDetail.TransactionErrorReason is null)
            {
                var ex = new InvalidOperationException("TransactionErrorReason is mandatory");
                ex.Data.Add("TrackingId", pending.TrackingId);
                throw ex;
            }

            await transactionWatcherService.SetToRegistryErrorAsync(
                pending.TrackingId,
                transactionDetail.TransactionErrorReason.Value);

            pending.SetStatusError();

            logger.TransactionWatcher(pending.TrackingId, transactionDetail.Successful);
        }

        private async Task TransactionExecutedSuccessAsync(
            TransactionPending pending,
            TransactionDetail transactionDetail)
        {
            if (transactionDetail.Successful.HasValue &&
                !transactionDetail.Successful.Value)
            {
                var ex = new InvalidOperationException("TransactionExecutedSuccess must be call only with successful TransactionDetail");
                ex.Data.Add("TrackingId", pending.TrackingId);
                throw ex;
            }

            pending.SetCompleted();
            await transactionWatcherService.SetToRegistryCompletedAsync(
                pending.TrackingId,
                transactionDetail);
            await transactionWatcherService.SetTransactionTriageCompletedAsync(pending.TrackingId);
            await transactionWatcherService.SetTransactionPoolCompletedAsync(pending.TrackingId);

            logger.TransactionWatcher(pending.TrackingId, transactionDetail.Successful);
        }
    }
}
