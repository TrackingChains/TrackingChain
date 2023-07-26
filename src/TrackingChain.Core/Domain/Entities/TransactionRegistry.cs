﻿using System;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionRegistry : TransactionBase
    {
        // Constructors.
        public TransactionRegistry(
            string code,
            string data,
            Guid trackingIdentify,
            long smartContractId,
            string smartContractAddress,
            Guid profileGroupId,
            int chainNumberId,
            ChainType chainType,
            DateTime triageDate)
            : base(code, data, chainNumberId, chainType, smartContractId, smartContractAddress, profileGroupId)
        {
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected TransactionRegistry() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid TrackingId { get; private set; }
        public TransactionStep TransactionStep { get; private set; }
        public DateTime TriageDate { get; private set; }
        public DateTime PendingDate { get; private set; }
        public DateTime PoolDate { get; private set; }
        public string? ReceiptBlockHash { get; private set; }
        public string? ReceiptBlockNumberHex { get; private set; }
        public string? ReceiptCumulativeGasUsedHex { get; private set; }
        public string? ReceiptEffectiveGasPriceHex { get; private set; }
        public string? ReceiptFrom { get; private set; }
        public string? ReceiptGasUsedHex { get; private set; }
        public string? ReceiptTo { get; private set; }
        public string? ReceiptTypeHex { get; private set; }
        public DateTime RegistryDate { get; private set; }

        // Methods.
        public void SetToPool()
        {
            TransactionStep = TransactionStep.Pool;
            PoolDate = DateTime.UtcNow;
        }
        
        public void SetToPending()
        {
            TransactionStep = TransactionStep.Pending;
            PendingDate = DateTime.UtcNow;
        }

        public void SetToRegistry(
            string receiptBlockHash,
            string receiptBlockNumberHex,
            string receiptCumulativeGasUsedHex,
            string receiptEffectiveGasPriceHex,
            string receiptFrom,
            string receiptGasUsedHex,
            string receiptTo,
            string receiptTypeHex)
        {
            TransactionStep = TransactionStep.Completed;
            ReceiptBlockHash = receiptBlockHash;
            ReceiptBlockNumberHex = receiptBlockNumberHex;
            ReceiptCumulativeGasUsedHex = receiptCumulativeGasUsedHex;
            ReceiptEffectiveGasPriceHex = receiptEffectiveGasPriceHex;
            ReceiptFrom = receiptFrom;
            ReceiptGasUsedHex = receiptGasUsedHex;
            ReceiptTo = receiptTo;
            ReceiptTypeHex = receiptTypeHex;
            RegistryDate = DateTime.UtcNow;
        }
    }
}