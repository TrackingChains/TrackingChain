using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrackingChain.Common.Dto
{
    public class TransactionDetail
    {
        public string TransactionHash { get; set; }
        //public HexBigInteger TransactionIndex { get; set; }
        public string BlockHash { get; set; }
        //public HexBigInteger BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        //public HexBigInteger CumulativeGasUsed { get; set; }
        //public HexBigInteger GasUsed { get; set; }
        //public HexBigInteger EffectiveGasPrice { get; set; }
        public string ContractAddress { get; set; }
        //public HexBigInteger Status { get; set; }
        //public HexBigInteger Type { get; set; }
    }
}
