﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;

namespace TrackingChain.AggregatorPoolCore.Services
{
    public class AggregatorService : IAggregatorService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<AggregatorService> logger;

        // Constructors.
        public AggregatorService(
            ApplicationDbContext applicationDbContext,
            ILogger<AggregatorService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public IEnumerable<TransactionPool> Enqueue(IEnumerable<TransactionTriage> triages)
        {
            ArgumentNullException.ThrowIfNull(triages);

            var transactionQueues = triages.Select(p => new TransactionPool(
                p.Code,
                p.DataValue,
                p.TrackingIdentify,
                p.ReceivedDate,
                p.SmartContractId,
                p.SmartContractAddress,
                p.SmartContractExtraInfo,
                p.ProfileGroupId,
                p.ChainNumberId,
                p.ChainType));
            applicationDbContext.AddRange(transactionQueues);

            foreach (var item in triages)
                item.SetInPool();

            applicationDbContext.UpdateRange(triages);

            return transactionQueues;
        }

        public async Task<IEnumerable<TransactionTriage>> GetTransactionToEnqueueAsync(
            int max,
            IEnumerable<Guid> profileIds)
        {
            var subquery = applicationDbContext.TransactionTriages
                .Where(p => p.Completed == false)
                .GroupBy(p => new { p.Code, p.SmartContractId })
                .Where(g => g.Max(x => x.IsInPool ? 1 : 0) == 0)
                .Take(max)
                .Select(g => g.Min(x => x.Id));

            var query = applicationDbContext.TransactionTriages
                        .Join(subquery, tt => tt.Id, id => id, (tt, id) => tt);

            /*
             * Further use.
            if (profileIds.Any())
            {
                
                var subqueryAccountProfiles = applicationDbContext.Accounts
                    .Where(ap => profileIds.Contains(ap.Id));
                query = query.Join(subqueryAccountProfiles, tt => tt.ProfileGroupId, ap => ap.Id, (tt, ap) => tt);
                
            }
            var test = query.ToQueryString();
            */

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TransactionRegistry>> SetToPoolAsync(IEnumerable<Guid> trackingIds)
        {
            ArgumentNullException.ThrowIfNull(trackingIds);

            var transactionRegistries = await applicationDbContext.TransactionRegistries
                .Where(tr => trackingIds.Contains(tr.TrackingId))
                .ToListAsync();

            //TODO Further use manage this case where any missing

            transactionRegistries.ForEach(tp => tp.SetToPool());

            applicationDbContext.UpdateRange(transactionRegistries);

            transactionRegistries.ForEach(tp => logger.TransactionInPool(
                tp.TrackingId,
                tp.SmartContractAddress,
                tp.ProfileGroupId));
            return transactionRegistries;
        }
    }
}
