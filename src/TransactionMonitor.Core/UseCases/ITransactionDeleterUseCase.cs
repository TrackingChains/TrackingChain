using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface ITransactionDeleterUseCase
    {
        Task<bool> RunAsync(int max);
    }
}
