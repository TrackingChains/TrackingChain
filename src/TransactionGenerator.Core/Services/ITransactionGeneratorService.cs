using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionGeneratorCore.Services
{
    public interface ITransactionGeneratorService
    {
        TransactionPending AddTransactionPendingFromPool(TransactionPool pool, string txHash);
        Task<IEnumerable<TransactionPool>> GetAvaiableTransactionPoolAsync(int max, Guid account);
        Task<TransactionRegistry> SetToPendingAsync(Guid trackingId);
    }
}
