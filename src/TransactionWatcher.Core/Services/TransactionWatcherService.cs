using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionWatcherCore.Services
{
    public class TransactionWatcherService : ITransactionWatcherService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionWatcherService> logger;

        // Constructors.
        public TransactionWatcherService(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionWatcherService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.

        public async Task<IEnumerable<TransactionPending>> GetTransactionToCheckAsync(
            int max,
            Guid account)
        {
            return await applicationDbContext.TransactionPendings
                .Join(applicationDbContext.AccountProfileGroup.Where(atg => atg.AccountId == account),
                      tp => tp.ProfileGroupId,
                      atg => atg.ProfileGroupId,
                      (tp, atg) => tp)
                .Where(p => p.Locked == false &&
                            p.WatchingFrom < DateTime.UtcNow)
                .Take(max)
                .OrderBy(tp => Guid.NewGuid())
                .ToListAsync();
        }

        public async Task<TransactionPool> SetTransactionPoolCompletedAsync(Guid trackingId)
        {
            var transactionPool = await applicationDbContext.TransactionPools
                .Where(tp => tp.TrackingId == trackingId)
                .FirstOrDefaultAsync();

            if (transactionPool is null)
            {
                var ex = new InvalidOperationException("TransactionPool not found");
                ex.Data.Add("TrackingId", trackingId);
                throw ex;
            }

            transactionPool.SetCompleted();
            applicationDbContext.Update(transactionPool);

            return transactionPool;
        }

        public async Task<TransactionTriage> SetTransactionTriageCompletedAsync(Guid trackingId)
        {
            var transactionTriage = await applicationDbContext.TransactionTriages
                .Where(tp => tp.TrackingIdentify == trackingId)
                .FirstOrDefaultAsync();

            if (transactionTriage is null)
            {
                var ex = new InvalidOperationException("transactionTriage not found");
                ex.Data.Add("TrackingId", trackingId);
                throw ex;
            }

            transactionTriage.SetCompleted();
            applicationDbContext.Update(transactionTriage);

            return transactionTriage;
        }

        public async Task<TransactionRegistry> SetToRegistryCompletedAsync(
            Guid trackingId,
            TransactionDetail transactionDetail)
        {
            ArgumentNullException.ThrowIfNull(transactionDetail);

            var transactionRegistry = await applicationDbContext.TransactionRegistries
                .FirstOrDefaultAsync(tr => tr.TrackingId == trackingId);

            if (transactionRegistry is null)
            {
                var ex = new InvalidOperationException("Registry not found");
                ex.Data.Add("TrackingId", transactionRegistry);
                throw ex;
            }

            transactionRegistry.SetToRegistryCompleted(
                transactionDetail.BlockHash,
                transactionDetail.BlockNumber,
                transactionDetail.CumulativeGasUsed,
                transactionDetail.EffectiveGasPrice,
                transactionDetail.From,
                transactionDetail.GasUsed,
                transactionDetail.Successful,
                transactionDetail.TransactionHash,
                transactionDetail.To);

            applicationDbContext.Update(transactionRegistry);

            return transactionRegistry;
        }

        public async Task<TransactionRegistry> SetToRegistryErrorAsync(
            Guid trackingId,
            TransactionErrorReason transactionErrorReason)
        {
            var transactionRegistry = await applicationDbContext.TransactionRegistries
                .FirstOrDefaultAsync(tr => tr.TrackingId == trackingId);

            if (transactionRegistry is null)
            {
                var ex = new InvalidOperationException("Registry not found");
                ex.Data.Add("TrackingId", transactionRegistry);
                throw ex;
            }

            transactionRegistry.SetToRegistryError(transactionErrorReason);

            applicationDbContext.Update(transactionRegistry);

            return transactionRegistry;
        }
    }
}
