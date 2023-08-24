using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface IAlertUseCase
    {
        Task<bool> RunAsync(int max);
    }
}
