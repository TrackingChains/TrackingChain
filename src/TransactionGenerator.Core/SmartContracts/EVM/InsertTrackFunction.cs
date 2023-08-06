using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace TrackingChain.TransactionGeneratorCore.SmartContracts.EVM
{
    [Function("InsertTrack")]
    public class InsertTrackFunction : FunctionMessage
    {
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Parameter("bytes32", "code", 1)]
        public virtual byte[] Code { get; set; }
        [Parameter("bytes", "dataValue", 2)]
        public virtual byte[] DataValue { get; set; }
        [Parameter("bool", "closed", 3)]
        public virtual bool Closed { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
