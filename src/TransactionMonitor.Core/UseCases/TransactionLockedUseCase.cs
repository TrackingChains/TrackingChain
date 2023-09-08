using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionMonitorCore.Services;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public class TransactionLockedUseCase : ITransactionLockedUseCase
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionLockedUseCase> logger;
        private readonly ITransactionMonitorService transactionMonitorService;

        // Constructors.
        public TransactionLockedUseCase(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionLockedUseCase> logger,
            ITransactionMonitorService transactionMonitorService)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
            this.transactionMonitorService = transactionMonitorService;
        }

        // Methods.
        public async Task<int> ReProcessAsync(
            int max,
            int unlockUncompletedGeneratorAfterSeconds,
            int unlockUncompletedWatcherAfterSeconds)
        {
            logger.StartReProcessTransactionLockedUseCase(max, unlockUncompletedGeneratorAfterSeconds, unlockUncompletedWatcherAfterSeconds);

            var pendings = await transactionMonitorService.GetPendingLockedInTimeoutAsync(max, unlockUncompletedWatcherAfterSeconds);

            IEnumerable<TransactionPool> pools = Array.Empty<TransactionPool>();
            if (pendings.Count() < max)
                pools = await transactionMonitorService.GetPoolLockedInTimeoutAsync(max - pendings.Count(), unlockUncompletedGeneratorAfterSeconds);

            foreach (var pending in pendings)
            {
                pending.UnlockFromError(TransactionErrorReason.LockedTimeOut, 0);
                applicationDbContext.Update(pending);

                var reportItem = new Core.Domain.Entities.ReportItem(
                $"Pending LockedDated: {pending.LockedDated} LockedBy: {pending.LockedBy}",
                0,
                false,
                ReportItemType.LockTimeOut,
                pending.TrackingId);
                applicationDbContext.Add(reportItem);
            }

            foreach (var pool in pools)
            {
                pool.UnlockFromError(TransactionErrorReason.LockedTimeOut, 0);
                applicationDbContext.Update(pool);

                var reportItem = new Core.Domain.Entities.ReportItem(
                $"Pool LockedDated: {pool.LockedDated} LockedBy: {pool.LockedBy}",
                0,
                false,
                ReportItemType.LockTimeOut,
                pool.TrackingId);
                applicationDbContext.Add(reportItem);
            }

            await applicationDbContext.SaveChangesAsync();

            var processed = pools.Count() + pendings.Count();
            logger.EndReProcessTransactionLockedUseCase(processed);
            return processed;
        }
    }
}
