using System.Text.Json;
using TrackingChain.Common.Enums;

namespace TrackingChain.Common.ExtraInfos
{
    public class ContractExtraInfo
    {
        // Properties.
        public ulong ByteWeight { get; set; } = default!;
        public string GetTrackSelectorValue { get; set; } = default!;
        public string InsertTrackSelectorValue { get; set; } = default!;
        public ulong InsertTrackBasicWeight { get; set; } = default!;
        public ulong InsertTrackProofSize { get; set; } = default!;
        public ulong InsertTrackRefTime { get; set; } = default!;
        public SupportedClient SupportedClient { get; set; } = default!;
        public int WaitingSecondsForWatcherTx { get; set; } = default!;
        public bool WaitingForResult { get; set; } = default!;

        // Methods.
        public static ContractExtraInfo FromJson(string json) => JsonSerializer.Deserialize<ContractExtraInfo>(json) ?? new ContractExtraInfo();
    }
}
