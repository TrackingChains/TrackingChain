using System;
using System.Threading.Tasks;

namespace TrackingChain.TransactionWatcherCore.UseCases
{
    public interface IPendingTransactionWatcherUseCase
    {
        Task<bool> CheckTransactionStatusAsync(
            int max, 
            Guid accountId,
            int reTryAfterSeconds,
            int errorAfterReTry);
    }
}
