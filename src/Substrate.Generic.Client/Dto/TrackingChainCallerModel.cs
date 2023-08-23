using System.Numerics;

namespace TrackingChain.Substrate.Generic.Client.Dto
{
    public class TrackingChainCallerModel
    {
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public byte[] DataHex { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CA1819 // Properties should not return arrays
        public BigInteger Value { get; set; }
        public ulong ProofSize { get; set; } = 125952;
        public ulong RefTime { get; set; } = 3951114240;
        public BigInteger StorageDepositLimit { get; set; } = 14000000000;
    }
}
