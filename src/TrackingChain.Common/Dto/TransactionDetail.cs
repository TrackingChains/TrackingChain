using System.Numerics;

namespace TrackingChain.Common.Dto
{
    public class TransactionDetail
    {
        public string TransactionHash { get; set; }
        //public HexBigInteger TransactionIndex { get; set; }
        public string BlockHash { get; set; }
        public string BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CumulativeGasUsed { get; set; }
        public string GasUsed { get; set; }
        public string EffectiveGasPrice { get; set; }
        public string ContractAddress { get; set; }
        //public HexBigInteger Status { get; set; }
        //public HexBigInteger Type { get; set; }
    }
}
