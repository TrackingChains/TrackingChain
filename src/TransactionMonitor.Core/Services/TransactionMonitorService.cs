using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public class TransactionMonitorService : ITransactionMonitorService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionMonitorService> logger;

        // Constructors.
        public TransactionMonitorService(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionMonitorService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public async Task<IEnumerable<TransactionPending>> GetPendingLockedInTimeoutAsync(
            int max, 
            int timeoutSeconds)
        {
            return await applicationDbContext.TransactionPendings
                .Where(tp => !tp.Completed &&
                             tp.Locked &&
                             tp.WatchingFrom.AddSeconds(timeoutSeconds) < DateTime.UtcNow)
                .OrderBy(tp => tp.WatchingFrom)
                .Take(max)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionPool>> GetPoolLockedInTimeoutAsync(
            int max, 
            int timeoutSeconds)
        {
            return await applicationDbContext.TransactionPools
                .Where(tp => !tp.Completed &&
                             tp.Locked &&
                             tp.GeneratingFrom.AddSeconds(timeoutSeconds) < DateTime.UtcNow)
                .OrderBy(tp => tp.GeneratingFrom)
                .Take(max)
                .ToListAsync();
        }
    }
}
