using System;
using TrackingChain.Common.Enums;

namespace TrackingChain.Common.Dto
{
    public class TransactionDetail
    {
        // Constructors.
        public TransactionDetail(bool undefined)
        {
            if (!undefined)
                throw new ArgumentException($"Constructor for undefined detail");

            BlockHash = "";
            BlockNumber = "";
            ContractAddress = "";
            CumulativeGasUsed = "";
            EffectiveGasPrice = "";
            Error = "";
            From = "";
            GasUsed = "";
            Status = TransactionDetailStatus.Undefined;
            TransactionHash = "";
            To = "";
        }

        public TransactionDetail(string txHash)
        {
            BlockHash = "";
            BlockNumber = "";
            ContractAddress = "";
            CumulativeGasUsed = "";
            EffectiveGasPrice = "";
            Error = "";
            From = "";
            GasUsed = "";
            WatchOnlyTx = true;
            Status = TransactionDetailStatus.Undefined;
            TransactionHash = txHash;
            To = "";
        }

        public TransactionDetail(TransactionErrorReason transactionErrorReason)
        {
            TransactionErrorReason = transactionErrorReason;

            BlockHash = "";
            BlockNumber = "";
            ContractAddress = "";
            CumulativeGasUsed = "";
            EffectiveGasPrice = "";
            Error = "";
            From = "";
            GasUsed = "";
            Status = TransactionDetailStatus.Failed;
            TransactionHash = "";
            To = "";
        }

        public TransactionDetail(
            TransactionErrorReason transactionErrorReason,
            string blockHash,
            string blockNumber,
            string contractAddress,
            string cumulativeGasUsed,
            string effectiveGasPrice,
            string error,
            string from,
            string gasUsed,
            string transactionHash,
            string to)
        {
            BlockHash = blockHash;
            BlockNumber = blockNumber;
            ContractAddress = contractAddress;
            CumulativeGasUsed = cumulativeGasUsed;
            EffectiveGasPrice = effectiveGasPrice;
            Error = error;
            From = from;
            GasUsed = gasUsed;
            Status = TransactionDetailStatus.Failed;
            TransactionHash = transactionHash;
            TransactionErrorReason = transactionErrorReason;
            To = to;
        }

        public TransactionDetail(
            string blockHash,
            string blockNumber,
            string contractAddress,
            string cumulativeGasUsed,
            string effectiveGasPrice,
            string error,
            string from,
            string gasUsed,
            bool successful,
            string transactionHash,
            string to)
        {
            if (!successful)
                throw new ArgumentException($"Constructor for TransactionErrorReason detail");

            BlockHash = blockHash;
            BlockNumber = blockNumber;
            ContractAddress = contractAddress;
            CumulativeGasUsed = cumulativeGasUsed;
            EffectiveGasPrice = effectiveGasPrice;
            Error = error;
            From = from;
            GasUsed = gasUsed;
            Status = TransactionDetailStatus.Success;
            TransactionHash = transactionHash;
            To = to;
        }

        // Properties.
        public string BlockHash { get; set; }
        public string BlockNumber { get; set; }
        public string ContractAddress { get; set; }
        public string CumulativeGasUsed { get; set; }
        public string EffectiveGasPrice { get; set; }
        public string Error { get; set; }
        public string From { get; set; }
        public string GasUsed { get; set; }
        public bool WatchOnlyTx { get; set; }
        public TransactionDetailStatus Status { get; set; }
        public TransactionErrorReason? TransactionErrorReason { get; set; }
        public string TransactionHash { get; set; }
        public string To { get; set; }
    }
}
