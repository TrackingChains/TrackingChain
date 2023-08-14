using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionWatcherCore.Services
{
    public interface ITransactionWatcherService
    {
        Task<IEnumerable<TransactionPending>> GetTransactionToCheckAsync(
            int max, 
            Guid account);
        Task<TransactionPool> SetTransactionPoolCompletedAsync(Guid trackingId);
        Task<TransactionTriage> SetTransactionTriageCompletedAsync(Guid trackingId); 
        Task<TransactionRegistry> SetToRegistryCompletedAsync(
            Guid trackingId,
            TransactionDetail transactionDetail);
        Task<TransactionRegistry> SetToRegistryErrorAsync(
            Guid trackingId,
            TransactionErrorReason transactionErrorReason);
    }
}
