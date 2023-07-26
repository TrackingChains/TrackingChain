using System.Text.Json;

namespace TrackingChain.Common.ExtraInfos
{
    public class SubstractContractExtraInfo
    {
        // Properties.
        public string InsertTrackSelectorValue { get; set; } = default!;

        // Methods.
        public static SubstractContractExtraInfo FromJson(string json) => JsonSerializer.Deserialize<SubstractContractExtraInfo>(json) ?? new SubstractContractExtraInfo();
    }
}
