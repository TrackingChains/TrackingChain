using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                            p.ReceivedDate < DateTime.UtcNow.AddSeconds(30)) //TODO set configurable values
                .Take(max)
                .OrderBy(tp => Guid.NewGuid())
                .ToListAsync();
        }

        public async Task SetTransactionPoolCompletedAsync(Guid trackingId)
        {
            var transactionPool = await applicationDbContext.TransactionPools
                .Where(tp => tp.TrackingId == trackingId)
                .FirstOrDefaultAsync();

            if (transactionPool is null)
                return;

            transactionPool.SetCompleted();
        }

        public async Task SetTransactionTriageCompletedAsync(Guid trackingId)
        {
            var transactionTriage = await applicationDbContext.TransactionTriages
                .Where(tp => tp.TrackingIdentify == trackingId)
                .FirstOrDefaultAsync();

            if (transactionTriage is null)
                return;

            transactionTriage.SetCompleted();
        }

        public async Task<TransactionRegistry> SetToRegistryAsync(
            TransactionPending transactionPending,
            TransactionReceipt transactionReceipt)
        {
            ArgumentNullException.ThrowIfNull(transactionReceipt);

            var transactionRegistry = await applicationDbContext.TransactionRegistries
                .FirstOrDefaultAsync(tr => tr.TrackingId == transactionPending.TrackingId);

            if (transactionRegistry is null)
                throw new InvalidOperationException(); //TODO manage this case

            transactionRegistry.SetToRegistry(
                transactionReceipt.BlockHash,
                transactionReceipt.BlockNumber.HexValue,
                transactionReceipt.CumulativeGasUsed.HexValue,
                transactionReceipt.EffectiveGasPrice.HexValue,
                transactionReceipt.From,
                transactionReceipt.GasUsed.HexValue,
                transactionReceipt.To,
                transactionReceipt.Type.HexValue);

            applicationDbContext.Update(transactionRegistry);

            return transactionRegistry;
        }

    }
}
