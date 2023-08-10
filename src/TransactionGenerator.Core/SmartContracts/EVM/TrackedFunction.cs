using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace TrackingChain.TransactionGeneratorCore.SmartContracts.EVM
{
    [Function("_trackedProducts", "bytes32")]
    public class TrackedFunction : FunctionMessage
    {
        [Parameter("bytes32", "", 1)]
#pragma warning disable CA1819 // Properties should not return arrays
        public virtual byte[] ReturnValue1 { get; set; } = default!;
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
