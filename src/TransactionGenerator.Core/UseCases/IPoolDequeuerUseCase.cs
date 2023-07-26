using System;
using System.Threading.Tasks;

namespace TrackingChain.TransactionGeneratorCore.UseCases
{
    public interface IPoolDequeuerUseCase
    {
        Task<bool> DequeueTransactionAsync(int max, Guid accountId);
    }
}
