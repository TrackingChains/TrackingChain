using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
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
        Task<TransactionRegistry> SetToRegistryAsync(
            Guid trackingId,
            TransactionDetail transactionDetail);
    }
}
