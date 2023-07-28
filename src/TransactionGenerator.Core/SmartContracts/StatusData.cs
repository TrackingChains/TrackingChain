using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace TrackingChain.TransactionGeneratorCore.SmartContracts
{
    public class StatusData
    {
        [Parameter("bytes", "dataValue", 1)]
#pragma warning disable CA1819 // Properties should not return arrays
        public virtual byte[] DataValue { get; set; } = default!;
#pragma warning restore CA1819 // Properties should not return arrays

        [Parameter("uint256", "timestamp", 2)]
        public virtual BigInteger Timestamp { get; set; }

        [Parameter("uint256", "blockNumber", 3)]
        public virtual BigInteger BlockNumber { get; set; }

        [Parameter("bool", "closed", 4)]
        public virtual bool Closed { get; set; }
    }
}
