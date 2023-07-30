using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.AggregatorPoolCore.Services
{
    public interface IAggregatorService
    {
        IEnumerable<TransactionPool> Enqueue(IEnumerable<TransactionTriage> triages);
        Task<IEnumerable<TransactionTriage>> GetTransactionToEnqueueAsync(
            int max, 
            IEnumerable<Guid> profileIds);
        Task<IEnumerable<TransactionRegistry>> SetToPoolAsync(IEnumerable<Guid> trackingIds);
    }
}
