using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public interface ITransactionMonitorService
    {
        Task<IEnumerable<TransactionRegistry>> GetTransactionInErrorAsync(int max);
        Task<IEnumerable<TransactionTriage>> GetTransactionTriageAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<TransactionPool>> GetTransactionPoolAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<TransactionPending>> GetTransactionPendingAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<TransactionPending>> GetPendingLockedInTimeoutAsync(int max, int timeoutSeconds);
        Task<IEnumerable<TransactionPool>> GetPoolLockedInTimeoutAsync(int max, int timeoutSeconds);
    }
}
