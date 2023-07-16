using Nethereum.ABI.FunctionEncoding.Attributes;

namespace TrackingChain.TransactionGeneratorCore.SmartContracts
{
    [FunctionOutput]
    public class TrackedOutputDTO : IFunctionOutputDTO
    {
        [Parameter("bytes32", "code", 1)]
#pragma warning disable CA1819 // Properties should not return arrays
        public virtual byte[] Code { get; set; } = default!;
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
