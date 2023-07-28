using System.Threading.Tasks;

namespace TrackingChain.AggregatorPoolCore.UseCases
{
    public interface IEnqueuerPoolUseCase
    {
        Task<int> EnqueueTransactionAsync(int max);
    }
}
