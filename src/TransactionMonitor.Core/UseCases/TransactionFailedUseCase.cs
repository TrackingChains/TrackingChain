using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionMonitorCore.Services;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public class TransactionFailedUseCase : ITransactionFailedUseCase
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionFailedUseCase> logger;
        private readonly ITransactionMonitorService transactionMonitorService;

        // Constructors.
        public TransactionFailedUseCase(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionFailedUseCase> logger,
            ITransactionMonitorService transactionMonitorService)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
            this.transactionMonitorService = transactionMonitorService;
        }

        // Methods.
        public async Task ManageAsync(
            int max,
            int failedReTryTimes)
        {
            logger.StartManageTransactionFailedUseCase(max, failedReTryTimes);

            var registries = await transactionMonitorService.GetTransactionWaitingToReProcessAsync(max);
            var triages = await transactionMonitorService.GetTransactionTriageAsync(registries.Select(r => r.TrackingId));
            var pools = await transactionMonitorService.GetTransactionPoolAsync(registries.Select(r => r.TrackingId));
            var pendings = await transactionMonitorService.GetTransactionPendingAsync(registries.Select(r => r.TrackingId));

            foreach (var registry in registries)
            {
                if (registry.ErrorTime < failedReTryTimes &&
                    registry.Status != RegistryStatus.WaitingToCancel)
                    continue;

                try
                {
                    ManageTransactionToCancel(triages, pools, pendings, registry);
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.ManageTransactionToCancelInError(registry.TrackingId, ex);
                    applicationDbContext.Entry(registry).State = EntityState.Unchanged;
                }
            }

            foreach (var registry in registries.Where(r => r.Status != RegistryStatus.CanceledDueToError))
            {
                try
                {
                    ManageTransactionToRetry(triages, pools, pendings, registry);
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.ManageTransactionToRetryInError(registry.TrackingId, ex);
                    applicationDbContext.Entry(registry).State = EntityState.Unchanged;
                }
            }
                
            await applicationDbContext.SaveChangesAsync();

            var cancelledCount = registries.Where(r => r.Status == RegistryStatus.CanceledDueToError).Count();
            logger.EndManageTransactionFailedUseCase(cancelledCount, registries.Count() - cancelledCount);
        }

        // Helpers.
        private void ManageTransactionToCancel(
            IEnumerable<TransactionTriage> triages, 
            IEnumerable<TransactionPool> pools, 
            IEnumerable<TransactionPending> pendings, 
            TransactionRegistry registry)
        {
            registry.SetToCanceled();
            applicationDbContext.Update(registry);

            var triage = triages.FirstOrDefault(t => t.TrackingIdentify == registry.TrackingId);
            if (triage is not null)
            {
                triage.SetCompleted();
                applicationDbContext.Update(triage);
            }

            var pool = pools.FirstOrDefault(p => p.TrackingId == registry.TrackingId);
            if (pool is not null)
            {
                pool.SetCompleted();
                applicationDbContext.Update(pool);
            }

            var pending = pendings.FirstOrDefault(p => p.TrackingId == registry.TrackingId);
            if (pending is not null)
            {
                pending.SetCompleted();
                applicationDbContext.Update(pending);
            }

            logger.ManageTransactionFailedCanceledDueToErrorUseCase(registry.TrackingId, registry.ErrorTime);
        }

        private void ManageTransactionToRetry(
                IEnumerable<TransactionTriage> triages,
                IEnumerable<TransactionPool> pools,
                IEnumerable<TransactionPending> pendings,
                TransactionRegistry registry)
        {
            var pool = pools.FirstOrDefault(p => p.TrackingId == registry.TrackingId);
            if (pool is null)
            {
                logger.ManageTransactionFailedUndefinedRecoveryUseCase(registry.TrackingId, registry.ErrorTime, "Pool NULL");
                registry.SetWaitingToCancel();
                return;
            }

            if (registry.TransactionStep == TransactionStep.Pool)
            {
                if (registry.TransactionErrorReason == TransactionErrorReason.UnableToSendTransactionOnChain ||
                    registry.TransactionErrorReason == TransactionErrorReason.InsertTransactionExpection)
                {
                    registry.Reprocessable(TransactionStep.Pool);
                    applicationDbContext.Update(registry);

                    pool.Reprocessable();
                    applicationDbContext.Update(pool);
                    logger.ManageTransactionFailedToReprocessableUseCase(registry.TrackingId, registry.ErrorTime, registry.TransactionErrorReason.Value);
                }
                else
                {
                    logger.ManageTransactionFailedUndefinedRecoveryUseCase(registry.TrackingId, registry.ErrorTime, "Pool Step with TransactionErrorReason not valid");
                    ManageTransactionToCancel(triages, pools, pendings, registry);
                    return;
                }
            }
            else if (registry.TransactionStep == TransactionStep.Pending)
            {
                var pending = pendings.FirstOrDefault(p => p.TrackingId == registry.TrackingId);
                if (pending is null)
                {
                    logger.ManageTransactionFailedUndefinedRecoveryUseCase(registry.TrackingId, registry.ErrorTime, "Pending NULL");
                    ManageTransactionToCancel(triages, pools, pendings, registry);
                    return;
                }

                if (registry.TransactionErrorReason == TransactionErrorReason.TransactionNotFound ||
                    registry.TransactionErrorReason == TransactionErrorReason.TransactionFinalizedInError)
                {
                    registry.Reprocessable(TransactionStep.Pool);
                    applicationDbContext.Update(registry);

                    pool.Reprocessable();
                    applicationDbContext.Update(pool);
                    applicationDbContext.Remove(pending);
                    logger.ManageTransactionFailedToReprocessableUseCase(registry.TrackingId, registry.ErrorTime, registry.TransactionErrorReason.Value);
                }
                else if (registry.TransactionErrorReason == TransactionErrorReason.GetTrasactionReceiptExpection ||
                         registry.TransactionErrorReason == TransactionErrorReason.UnableToWatchTransactionOnChain)
                {
                    registry.Reprocessable(TransactionStep.Pending);
                    applicationDbContext.Update(registry);

                    pending.Reprocessable();
                    applicationDbContext.Update(pending);
                    logger.ManageTransactionFailedToReprocessableUseCase(registry.TrackingId, registry.ErrorTime, registry.TransactionErrorReason.Value);
                }
                else
                {
                    logger.ManageTransactionFailedUndefinedRecoveryUseCase(registry.TrackingId, registry.ErrorTime, "Pending Step with TransactionErrorReason not valid");
                    ManageTransactionToCancel(triages, pools, pendings, registry);
                    return;
                }
            }
            else
            {
                logger.ManageTransactionFailedUndefinedRecoveryUseCase(registry.TrackingId, registry.ErrorTime, $"Transaction Step Invalid {registry.TransactionStep}");
                return;
            }
        }
    }
}
