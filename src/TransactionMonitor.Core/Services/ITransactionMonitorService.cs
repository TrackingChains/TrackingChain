using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public interface ITransactionMonitorService
    {
        Task<IEnumerable<TransactionPending>> GetPendingLockedInTimeoutAsync(int max, int timeoutSeconds);
        Task<IEnumerable<TransactionPool>> GetPoolLockedInTimeoutAsync(int max, int timeoutSeconds);
    }
}
