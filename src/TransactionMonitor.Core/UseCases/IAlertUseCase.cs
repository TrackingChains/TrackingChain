using System;
using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface IAlertUseCase
    {
        Task<bool> RunAsync(
            TimeSpan intervalBetweenTransactionCancelledReport,
            TimeSpan intervalBetweenTransactionErrorReport);
    }
}
