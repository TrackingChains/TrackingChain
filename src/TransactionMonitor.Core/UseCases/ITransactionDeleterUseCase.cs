using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface ITransactionDeleterUseCase
    {
        Task<int> RunAsync(int max);
    }
}
