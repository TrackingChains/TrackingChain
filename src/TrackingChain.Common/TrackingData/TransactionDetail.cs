using System;

namespace TrackingChain.Common.Dto
{
    public class TransactionDetail
    {
        // Constructors.
        public TransactionDetail(bool undefined)
        {
            if (!undefined)
                throw new ArgumentException($"Constrcutor for undefined detail");

            BlockHash = "";
            BlockNumber = "";
            ContractAddress = "";
            CumulativeGasUsed = "";
            EffectiveGasPrice = "";
            Error = "";
            From = "";
            GasUsed = "";
            Undefined = undefined;
            Successful = null;
            TransactionHash = "";
            To = "";
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
            BlockHash = blockHash;
            BlockNumber = blockNumber;
            ContractAddress = contractAddress;
            CumulativeGasUsed = cumulativeGasUsed;
            EffectiveGasPrice = effectiveGasPrice;
            Error = error;
            From = from;
            GasUsed = gasUsed;
            Undefined = false;
            Successful = successful;
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
        public bool Undefined { get; set; }
        public bool? Successful { get; set; }
        public string TransactionHash { get; set; }
        public string To { get; set; }
    }
}
