﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
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
        public async Task ReProcessAsync(
            int max, 
            int unlockTimeoutSeconds)
        {
            var pendings = await transactionMonitorService.GetPendingLockedInTimeoutAsync(max, unlockTimeoutSeconds);

            IEnumerable<TransactionPool> pools = Array.Empty<TransactionPool>();
            if (pendings.Count() < max)
                pools = await transactionMonitorService.GetPoolLockedInTimeoutAsync(max - pendings.Count(), unlockTimeoutSeconds);

            foreach (var pending in pendings)
                pending.Unlock(0);
            applicationDbContext.UpdateRange(pendings);

            foreach (var pool in pools)
                pool.Unlock(0);
            applicationDbContext.UpdateRange(pools);

            await applicationDbContext.SaveChangesAsync();
        }
    }
}