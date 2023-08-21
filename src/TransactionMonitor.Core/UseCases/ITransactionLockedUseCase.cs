using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface ITransactionLockedUseCase
    {
        Task<int> ReProcessAsync(
            int max, 
            int unlockTimeoutSeconds);
    }
}
