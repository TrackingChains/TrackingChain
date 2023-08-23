namespace TrackingChain.Common.Dto
{
    public class DataDetail
    {
#pragma warning disable CA1819 // Properties should not return arrays
        public virtual byte[] DataValue { get; set; } = default!;
#pragma warning restore CA1819 // Properties should not return arrays
        public virtual long Timestamp { get; set; }
        public virtual long BlockNumber { get; set; }
    }
}
