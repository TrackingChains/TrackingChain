using System.Text.Json;
using TrackingChain.Common.Enums;

namespace TrackingChain.Common.ExtraInfos
{
    public class SubstractContractExtraInfo
    {
        // Properties.
        public ulong BasicWeight { get; set; } = default!;
        public ulong ByteWeight { get; set; } = default!;
        public string InsertTrackSelectorValue { get; set; } = default!;
        public ulong ProofSize { get; set; } = default!;
        public ulong RefTime { get; set; } = default!; 
        public SupportedClient SupportedClient { get; set; } = default!;

        // Methods.
        public static SubstractContractExtraInfo FromJson(string json) => JsonSerializer.Deserialize<SubstractContractExtraInfo>(json) ?? new SubstractContractExtraInfo();
    }
}
