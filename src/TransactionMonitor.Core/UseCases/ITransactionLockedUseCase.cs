using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface ITransactionLockedUseCase
    {
        Task ReProcessAsync(
            int max, 
            int unlockTimeoutSeconds);
    }
}
