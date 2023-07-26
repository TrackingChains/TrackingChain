using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionGeneratorCore.Services
{
    public class TransactionGeneratorService : ITransactionGeneratorService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionGeneratorService> logger;

        // Constructors.
        public TransactionGeneratorService(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionGeneratorService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        public TransactionPending AddTransactionPendingFroomPool(
            TransactionPool pool, 
            string txHash)
        {
            ArgumentNullException.ThrowIfNull(pool);

            var transactionPending = new TransactionPending(
                txHash,
                pool.Code,
                pool.Data,
                pool.ReceivedDate,
                pool.TrackingId,
                pool.TriageDate,
                pool.ProfileGroupId,
                pool.SmartContractId,
                pool.SmartContractAddress,
                pool.ChainNumberId,
                pool.ChainType);

            applicationDbContext.TransactionPendings.Add(transactionPending);

            return transactionPending;
        }

        // Methods.
        public async Task<IEnumerable<TransactionPool>> GetAvaiableTransactionPoolAsync(
            int max, 
            Guid account)
        {
            return await applicationDbContext.TransactionPools
                .Join(applicationDbContext.AccountProfileGroup.Where(atg => atg.AccountId == account),
                      tp => tp.ProfileGroupId,
                      atg => atg.ProfileGroupId,
                      (tp, atg) => tp)
                .Where(x => !x.Locked)
                .Take(max)
                .OrderBy(tp => Guid.NewGuid())
                .ToListAsync();
        }

        public async Task<TransactionRegistry> SetToPendingAsync(TransactionPending transactionPending)
        {
            ArgumentNullException.ThrowIfNull(transactionPending);

            var transactionRegistry = await applicationDbContext.TransactionRegistries
                .FirstOrDefaultAsync(tr => tr.TrackingId == transactionPending.TrackingId);

            if (transactionRegistry is null)
                throw new InvalidOperationException(); //TODO manage this case

            transactionRegistry.SetToPending();

            applicationDbContext.Update(transactionRegistry);

            return transactionRegistry;
        }
    }
}
