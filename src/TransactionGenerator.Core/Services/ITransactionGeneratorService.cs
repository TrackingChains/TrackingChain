﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionGeneratorCore.Services
{
    public interface ITransactionGeneratorService
    {
        TransactionPending AddTransactionPendingFromPool(
            TransactionPool pool, 
            string txHash);
        Task<IEnumerable<TransactionPool>> GetAvaiableTransactionPoolAsync(
            int max, 
            Guid account);
        Task<TransactionTriage> SetTransactionTriageCompletedAsync(Guid trackingId);
        Task<TransactionRegistry> SetToPendingAsync(
            Guid trackingId, 
            string lastTransactionHash, 
            string smartContractEndpoint);
        Task<TransactionRegistry> SetToRegistryCompletedAsync(
            Guid trackingId,
            TransactionDetail transactionDetail);
        Task<TransactionRegistry> SetToRegistryErrorAsync(
            Guid trackingId,
            TransactionDetail transactionDetail);
        Task<TransactionTriage> SetTransactionTriageErrorCompletedAsync(Guid trackingId);
    }
}
