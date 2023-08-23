using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace TrackingChain.EVM.Generic.Client.Model
{
    [Function("GetCodeTracking", typeof(GetCodeTrackingOutputDTO))]
    public class GetCodeTrackingFunction : FunctionMessage
    {
        [Parameter("bytes32", "code", 1)]
#pragma warning disable CA1819 // Properties should not return arrays
        public virtual byte[] Code { get; set; } = default!;
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
