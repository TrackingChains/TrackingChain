using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace TrackingChain.TransactionGeneratorCore.SmartContracts.EVM
{

    [FunctionOutput]
    public class GetCodeTrackingOutputDTO : IFunctionOutputDTO
    {
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Parameter("tuple[]", "", 1)]
        public virtual List<StatusData> ReturnValue1 { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists
    }
}
