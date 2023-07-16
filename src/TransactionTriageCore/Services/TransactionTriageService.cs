using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionTriageCore.Services;

namespace TrackingChain.TransactionWaitingCore.Services
{
    public class TransactionTriageService : ITransactionTriageService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionTriageService> logger;
        private readonly IRegistryService registryService;

        // Constructors.
        public TransactionTriageService(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionTriageService> logger,
            IRegistryService registryService)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
            this.registryService = registryService;
        }

        // Methods.
        public TransactionRegistry AddRegistry(TransactionTriage transactionTriage)
        {
            ArgumentNullException.ThrowIfNull(transactionTriage);

            var transactionRegistry = new TransactionRegistry(
                transactionTriage.Code,
                transactionTriage.Data,
                transactionTriage.TrackingIdentify,
                transactionTriage.SmartContractId,
                transactionTriage.SmartContractAddress,
                transactionTriage.ProfileGroupId,
                transactionTriage.ChainNumberId,
                transactionTriage.ChainType,
                transactionTriage.ReceivedDate);

            applicationDbContext.Add(transactionRegistry);

            return transactionRegistry;
        }

        public async Task<TransactionTriage> AddTransactionAsync(string code, string category, string data)
        {
            var group = await GetSmartContractForTransactionAsync("testAuth", code, category); //TODO dynamic testAuth
            var smartContract = await registryService.GetSmartContractAsync(group.SmartContractId);

            var transactionTriage = new TransactionTriage(
                code, 
                data,
                group.Id,
                smartContract.Id,
                smartContract.Address,
                smartContract.ChainNumberId,
                smartContract.ChainType);

            applicationDbContext.Add(transactionTriage);

            return transactionTriage;
        }

        public async Task<ProfileGroup> GetSmartContractForTransactionAsync(
            string authority, 
            string code, 
            string category)
        {
            var transactionGroups = await applicationDbContext.ProfileGroups
                                                .OrderBy(scp => scp.Priority)
                                                .Where(scp => (scp.Authority == authority || scp.Authority == null) &&
                                                              (scp.Category == category || scp.Category == null))
                                                .ToListAsync();

            var groupWithAggrCodes = transactionGroups.Where(scp => !string.IsNullOrWhiteSpace(scp.AggregationCode));
            foreach(var group in groupWithAggrCodes) { 
                //TODO first group match return current profile.
            }

            var groupNoAggrCodes = transactionGroups.Where(scp => string.IsNullOrWhiteSpace(scp.AggregationCode));
            if (!groupNoAggrCodes.Any())
            {
                var ex = new InvalidOperationException("Smart contract group not found");
                ex.Data.Add("Authority", authority);
                ex.Data.Add("Code", code);
                ex.Data.Add("Category", category);
                throw ex;
            } 
                
            return groupNoAggrCodes.First();
        }

        public async Task<List<TransactionTriage>> GetTransactionReadyForPoolAsync(int max = 100)
        {
            //var result = applicationDbContext.Pools
            //                .Where(pool => applicationDbContext.Pools
            //                    .Where(p => p.Executed == false)
            //                    .GroupBy(p => p.Code)
            //                    .Where(g => g.Max(x => x.Queued) == false)
            //                    .Select(g => g.Min(x => x.Id))
            //                    .Contains(pool.Id))
            //                .ToListAsync();
            var subquery = applicationDbContext.TransactionTriages
                                .Where(p => p.Completed == false)
                                .GroupBy(p => p.Code)
                                .Where(g => g.Max(x => x.IsInPool) == false)
                                .Take(max)
                                .Select(g => g.Min(x => x.Id));

            var query = applicationDbContext.TransactionTriages
                        .Join(subquery, p => p.Id, id => id, (p, id) => p);
            var test = query.ToQueryString();
            return await query.ToListAsync();
        }
    }
}
