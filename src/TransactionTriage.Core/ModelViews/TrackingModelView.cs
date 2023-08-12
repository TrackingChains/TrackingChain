using System;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TransactionTriageCore.ModelViews
{
    public class TrackingModelView
    {
        protected TrackingModelView(
            string code,
            string dataValue,
            string? lastTransactionHash,
            Guid trackingIdentify,
            long smartContractId,
            string smartContractAddress,
            string? smartContractEndpoint,
            string smartContractExtraInfo,
            Guid profileGroupId,
            int chainNumberId,
            ChainType chainType,
            DateTime triageDate,
            DateTime registryDate,
            DateTime pendingDate,
            DateTime poolDate,
            string? receiptBlockHash,
            string? receiptBlockNumber,
            string? receiptCumulativeGasUsed,
            string? receiptEffectiveGasPrice,
            string? receiptFrom,
            string? receiptGasUsed,
            bool? receiptSuccessful,
            string? receiptTransactionHash,
            string? receiptTo,
            TransactionStep transactionStep)
        {
            Code = code;
            DataValue = dataValue;
            ChainNumberId = chainNumberId;
            ChainType = chainType;
            LastTransactionHash = lastTransactionHash;
            SmartContractExtraInfo = smartContractExtraInfo;
            ReceivedDate = DateTime.UtcNow;
            SmartContractId = smartContractId;
            SmartContractAddress = smartContractAddress;
            SmartContractEndpoint = smartContractEndpoint;
            ProfileGroupId = profileGroupId;
            RegistryDate = registryDate;
            PendingDate = pendingDate;
            PoolDate = poolDate;
            ReceiptBlockHash = receiptBlockHash;
            ReceiptBlockNumber = receiptBlockNumber;
            ReceiptCumulativeGasUsed = receiptCumulativeGasUsed;
            ReceiptEffectiveGasPrice = receiptEffectiveGasPrice;
            ReceiptFrom = receiptFrom;
            ReceiptGasUsed = receiptGasUsed;
            ReceiptSuccessful = receiptSuccessful;
            ReceiptTransactionHash = receiptTransactionHash;
            ReceiptTo = receiptTo;
            TrackingId = trackingIdentify;
            TransactionStep = transactionStep;
            TriageDate = triageDate;
        }

        // Properties.
        public string Code { get; protected set; }
        public int ChainNumberId { get; protected set; }
        public ChainType ChainType { get; protected set; }
        public string DataValue { get; protected set; }
        public string? LastTransactionHash { get; private set; }
        public DateTime ReceivedDate { get; protected set; }
        public string? ReceiptBlockHash { get; private set; }
        public string? ReceiptBlockNumber { get; private set; }
        public string? ReceiptCumulativeGasUsed { get; private set; }
        public string? ReceiptEffectiveGasPrice { get; private set; }
        public string? ReceiptFrom { get; private set; }
        public string? ReceiptGasUsed { get; private set; }
        public bool? ReceiptSuccessful { get; private set; }
        public string? ReceiptTo { get; private set; }
        public string? ReceiptTransactionHash { get; private set; }
        public DateTime RegistryDate { get; private set; }
        public string SmartContractExtraInfo { get; private set; }
        public long SmartContractId { get; protected set; }
        public string SmartContractAddress { get; protected set; }
        public string? SmartContractEndpoint { get; protected set; }
        public Guid TrackingId { get; private set; }
        public TransactionStep TransactionStep { get; private set; }
        public DateTime TriageDate { get; private set; }
        public DateTime PendingDate { get; private set; }
        public DateTime PoolDate { get; private set; }
        public Guid ProfileGroupId { get; private set; }

        // Methods.
        public static TrackingModelView FromEntity(TransactionRegistry transactionRegistry)
        {
            ArgumentNullException.ThrowIfNull(transactionRegistry);

            return new TrackingModelView(
                transactionRegistry.Code,
                transactionRegistry.DataValue,
                transactionRegistry.LastTransactionHash,
                transactionRegistry.TrackingId,
                transactionRegistry.SmartContractId,
                transactionRegistry.SmartContractAddress,
                transactionRegistry.GetFirstRandomEndpointAddress,
                transactionRegistry.SmartContractExtraInfo,
                transactionRegistry.ProfileGroupId,
                transactionRegistry.ChainNumberId,
                transactionRegistry.ChainType,
                transactionRegistry.TriageDate,
                transactionRegistry.RegistryDate,
                transactionRegistry.PendingDate,
                transactionRegistry.PoolDate,
                transactionRegistry.ReceiptBlockHash,
                transactionRegistry.ReceiptBlockNumber,
                transactionRegistry.ReceiptCumulativeGasUsed,
                transactionRegistry.ReceiptEffectiveGasPrice,
                transactionRegistry.ReceiptFrom,
                transactionRegistry.ReceiptGasUsed,
                transactionRegistry.ReceiptSuccessful,
                transactionRegistry.ReceiptTransactionHash,
                transactionRegistry.ReceiptTo,
                transactionRegistry.TransactionStep);
        }

        public static TrackingModelView FromEntity(TransactionTriage transactionTriage)
        {
            ArgumentNullException.ThrowIfNull(transactionTriage);

            return new TrackingModelView(
                transactionTriage.Code,
                transactionTriage.DataValue,
                null,
                transactionTriage.TrackingIdentify,
                transactionTriage.SmartContractId,
                transactionTriage.SmartContractAddress,
                null,
                transactionTriage.SmartContractExtraInfo,
                transactionTriage.ProfileGroupId,
                transactionTriage.ChainNumberId,
                transactionTriage.ChainType,
                transactionTriage.ReceivedDate,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                TransactionStep.Triage);
        }

        public static TrackingModelView FromEntity(TransactionPool transactionPool)
        {
            ArgumentNullException.ThrowIfNull(transactionPool);

            return new TrackingModelView(
                transactionPool.Code,
                transactionPool.DataValue,
                null,
                transactionPool.TrackingId,
                transactionPool.SmartContractId,
                transactionPool.SmartContractAddress,
                null,
                transactionPool.SmartContractExtraInfo,
                transactionPool.ProfileGroupId,
                transactionPool.ChainNumberId,
                transactionPool.ChainType,
                transactionPool.TriageDate,
                DateTime.MinValue,
                DateTime.MinValue,
                transactionPool.ReceivedDate,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                TransactionStep.Pool);
        }

        public static TrackingModelView FromEntity(TransactionPending transactionPending)
        {
            ArgumentNullException.ThrowIfNull(transactionPending);

            return new TrackingModelView(
                transactionPending.Code,
                transactionPending.DataValue,
                transactionPending.TxHash,
                transactionPending.TrackingId,
                transactionPending.SmartContractId,
                transactionPending.SmartContractAddress,
                null,
                transactionPending.SmartContractExtraInfo,
                transactionPending.ProfileGroupId,
                transactionPending.ChainNumberId,
                transactionPending.ChainType,
                transactionPending.TriageDate,
                DateTime.MinValue,
                transactionPending.ReceivedDate,
                DateTime.MinValue,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                TransactionStep.Pending);
        }
    }
}
