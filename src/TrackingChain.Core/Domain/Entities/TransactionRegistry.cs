using System;
using System.Linq;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;
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
            Status = RegistryStatus.InProgress;
        }
        protected TransactionRegistry() { }

        // Properties.
        public int ErrorTime { get; private set; }
        public string? LastTransactionHash { get; private set; }
        public Guid TrackingId { get; private set; }
        public TransactionStep TransactionStep { get; private set; }
        public TransactionErrorReason? TransactionErrorReason { get; set; }
        public DateTime TriageDate { get; private set; }
        public DateTime PendingDate { get; private set; }
        public DateTime PoolDate { get; private set; }
        public string? ReceiptBlockHash { get; private set; }
        public string? ReceiptBlockNumber { get; private set; }
        public string? ReceiptCumulativeGasUsed { get; private set; }
        public string? ReceiptEffectiveGasPrice { get; private set; }
        public string? ReceiptFrom { get; private set; }
        public string? ReceiptGasUsed { get; private set; }
        public bool ReceiptReceived { get; private set; }
        public string? ReceiptTransactionHash { get; private set; }
        public string? ReceiptTo { get; private set; }
        public DateTime RegistryDate { get; private set; }
        public string? SmartContractEndpoint { get; protected set; }
        public RegistryStatus Status { get; protected set; }

        public string? GetFirstRandomEndpointAddress
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SmartContractEndpoint))
                    return null;

                var address = SmartContractEndpoint.Split(";");
                if (address.Length == 1)
                    return address.First();

                var rnd = new Random();
#pragma warning disable CA5394 // No need secure number
                return address[rnd.Next(address.Length)];
#pragma warning restore CA5394 // No need secure number
            }
        }

        // Methods.
        public void Reprocessable(TransactionStep restartFromStep)
        {
            TransactionStep = restartFromStep;
            Status = RegistryStatus.InProgress;
        }
        
        public void SetToPool()
        {
            TransactionStep = TransactionStep.Pool;
            PoolDate = DateTime.UtcNow;
        }

        public void SetToPending(
            string lastTransactionHash,
            string smartContractEndpoint)
        {
            LastTransactionHash = lastTransactionHash;
            SmartContractEndpoint = smartContractEndpoint;
            TransactionStep = TransactionStep.Pending;
            PendingDate = DateTime.UtcNow;
        }

        public void SetToCanceled()
        {
            TransactionStep = TransactionStep.Completed;
            Status = RegistryStatus.CanceledDueToError;
        }

        public void SetToRegistryCompleted(
            string receiptBlockHash,
            string receiptBlockNumber,
            string receiptCumulativeGasUsed,
            string receiptEffectiveGasPrice,
            string receiptFrom,
            string receiptGasUsed,
            bool? receiptSuccessful,
            string receiptTransactionHash,
            string receiptTo)
        {
            if (receiptSuccessful.HasValue &&
                !receiptSuccessful.Value)
                throw new InvalidOperationException("use SetToRegistryError");

            TransactionStep = TransactionStep.Completed;
            TransactionErrorReason = null;
            ReceiptBlockHash = receiptBlockHash;
            ReceiptBlockNumber = receiptBlockNumber;
            ReceiptCumulativeGasUsed = receiptCumulativeGasUsed;
            ReceiptEffectiveGasPrice = receiptEffectiveGasPrice;
            ReceiptFrom = receiptFrom;
            ReceiptGasUsed = receiptGasUsed;
            ReceiptTransactionHash = receiptTransactionHash;
            ReceiptTo = receiptTo;
            RegistryDate = DateTime.UtcNow;
            ReceiptReceived = receiptSuccessful.HasValue;
            Status = RegistryStatus.SuccessfullyCompleted;
        }

        public void SetToRegistryError(TransactionErrorReason transactionErrorReason)
        {
            ErrorTime++;
            Status = RegistryStatus.Error;
            TransactionErrorReason = transactionErrorReason;
        }
    }
}
