using System;
using System.Threading.Tasks;

namespace TrackingChain.TransactionGeneratorCore.UseCases
{
    public interface IPoolDequeuerUseCase
    {
        Task<Guid> DequeueTransactionAsync(
            int max, 
            Guid accountId,
            int reTryAfterSeconds,
            int maxErrorTime);
    }
}
