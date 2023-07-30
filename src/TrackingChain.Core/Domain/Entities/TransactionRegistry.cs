using System;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionRegistry : TransactionBase
    {
        // Constructors.
        public TransactionRegistry(
            string code,
            string dataValue,
            Guid trackingIdentify,
            long smartContractId,
            string smartContractAddress,
            string smartContractExtraInfo,
            Guid profileGroupId,
            int chainNumberId,
            ChainType chainType,
            DateTime triageDate)
            : base(code, dataValue, chainNumberId, chainType, smartContractId, smartContractAddress, smartContractExtraInfo, profileGroupId)
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
        public string? ReceiptBlockNumber { get; private set; }
        public string? ReceiptCumulativeGasUsed { get; private set; }
        public string? ReceiptEffectiveGasPrice { get; private set; }
        public string? ReceiptFrom { get; private set; }
        public string? ReceiptGasUsed { get; private set; }
        public string? ReceiptTo { get; private set; }
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
            string receiptBlockNumber,
            string receiptCumulativeGasUsed,
            string receiptEffectiveGasPrice,
            string receiptFrom,
            string receiptGasUsed,
            string receiptTo)
        {
            TransactionStep = TransactionStep.Completed;
            ReceiptBlockHash = receiptBlockHash;
            ReceiptBlockNumber = receiptBlockNumber;
            ReceiptCumulativeGasUsed = receiptCumulativeGasUsed;
            ReceiptEffectiveGasPrice = receiptEffectiveGasPrice;
            ReceiptFrom = receiptFrom;
            ReceiptGasUsed = receiptGasUsed;
            ReceiptTo = receiptTo;
            RegistryDate = DateTime.UtcNow;
        }
    }
}
