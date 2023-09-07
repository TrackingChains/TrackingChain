using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
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
                pending.UnlockFromError(TransactionErrorReason.LockedTimeOut, 0);
            applicationDbContext.UpdateRange(pendings);

            foreach (var pool in pools)
                pool.UnlockFromError(TransactionErrorReason.LockedTimeOut, 0);
            applicationDbContext.UpdateRange(pools);

            await applicationDbContext.SaveChangesAsync();

            var processed = pools.Count() + pendings.Count();
            logger.EndReProcessTransactionLockedUseCase(processed);
            return processed;
        }
    }
}
