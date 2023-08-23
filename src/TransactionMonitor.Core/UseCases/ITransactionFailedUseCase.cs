using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface ITransactionFailedUseCase
    {
        Task ManageAsync(
            int max,
            int failedReTryTimes);
    }
}
