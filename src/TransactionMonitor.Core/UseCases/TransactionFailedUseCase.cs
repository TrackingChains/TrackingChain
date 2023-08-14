using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
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
            var registries = await transactionMonitorService.GetTransactionInErrorAsync(max);
            var triages = await transactionMonitorService.GetTransactionTriageAsync(registries.Select(r => r.TrackingId));
            var pools = await transactionMonitorService.GetTransactionPoolAsync(registries.Select(r => r.TrackingId));
            var pendings = await transactionMonitorService.GetTransactionPendingAsync(registries.Select(r => r.TrackingId));

            foreach (var registry in registries)
            {
                if (registry.ErrorTime < failedReTryTimes)
                    continue;

                ManageTransactionToCancel(triages, pools, pendings, registry);
            }

            foreach (var registry in registries.Where(r => r.Status != RegistryStatus.CanceledDueToError))
            {
                ManageTransactionToRetry(pools, pendings, registry);
            }

            await applicationDbContext.SaveChangesAsync();
        }

        // Helpers.
        private void ManageTransactionToCancel(IEnumerable<TransactionTriage> triages, IEnumerable<TransactionPool> pools, IEnumerable<TransactionPending> pendings, TransactionRegistry registry)
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
        }

        private void ManageTransactionToRetry(
                IEnumerable<TransactionPool> pools,
                IEnumerable<TransactionPending> pendings,
                TransactionRegistry registry)
        {
            var pool = pools.FirstOrDefault(p => p.TrackingId == registry.TrackingId);
            if (pool is null)
            {
                registry.SetToCanceled();
                //TODO log unable to manage this status. (missing pool item)
                //TODO maybe check the triage status
                return;
            }

            if (registry.TransactionStep == TransactionStep.Pool)
            {
                if (registry.TransactionErrorReason == TransactionErrorReason.UnableToSendTransactionOnChain)
                {
                    registry.Reprocessable(TransactionStep.Pool);
                    pool.Reprocessable();
                    applicationDbContext.Update(pool);
                }
                else
                {
                    //TODO log unable to manage this status. log case and try to investigate
                }
            }
            else if (registry.TransactionStep == TransactionStep.Pending)
            {
                var pending = pendings.FirstOrDefault(p => p.TrackingId == registry.TrackingId);
                if (pending is null)
                {
                    return;
                    //TODO recreate pending
                    //pending = new TransactionPending();
                    //LOG the problem as warning
                }

                if (registry.TransactionErrorReason == TransactionErrorReason.TransactionNotFound ||
                    registry.TransactionErrorReason == TransactionErrorReason.TransactionFinalizedInError)
                {
                    registry.Reprocessable(TransactionStep.Pool);
                    pool.Reprocessable();
                    applicationDbContext.Update(pool);
                    applicationDbContext.Remove(pending);
                }
                else if (registry.TransactionErrorReason == TransactionErrorReason.GetTrasactionReceiptExpection)
                {
                    registry.Reprocessable(TransactionStep.Pending);
                    pending.Reprocessable();
                }
                else
                {
                    //TODO log unable to manage this status. log case and try to investigate
                }
            }


            //TODO all cancelled registry to Completed in each Tables TX
        }
    }
}
