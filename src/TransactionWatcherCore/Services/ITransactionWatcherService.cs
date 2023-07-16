using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionWatcherCore.Services
{
    public interface ITransactionWatcherService
    {
        Task<IEnumerable<TransactionPending>> GetTransactionToCheckAsync(
            int max, 
            Guid account);
        Task SetTransactionPoolCompletedAsync(Guid trackingId);
        Task SetTransactionTriageCompletedAsync(Guid trackingId); 
        Task<TransactionRegistry> SetToRegistryAsync(
            TransactionPending transactionPending,
            TransactionReceipt transactionReceipt);
    }
}
