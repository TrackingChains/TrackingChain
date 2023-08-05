namespace TrackingChain.Core.Dto
{
#pragma warning disable IDE1006 // Naming Styles for Dto response
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. for Dto response
    public class WatcherResponseDto
    {
        public int code { get; set; }
        public string message { get; set; }
        public int generated_at { get; set; }
        public DataChain data { get; set; }
    }

    public class DataChain
    {
        public int block_timestamp { get; set; }
        public int block_num { get; set; }
        public string extrinsic_index { get; set; }
        public string account_id { get; set; }
        public string extrinsic_hash { get; set; }
        public bool success { get; set; }
        public string fee { get; set; }
        public string fee_used { get; set; }
        public object error { get; set; }
        public bool finalized { get; set; }
        public string block_hash { get; set; }
        public bool pending { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
