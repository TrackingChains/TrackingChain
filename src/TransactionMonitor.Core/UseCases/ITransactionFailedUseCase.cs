using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface ITransactionFailedUseCase
    {
        Task ReProcessAsync(int max);
    }
}
