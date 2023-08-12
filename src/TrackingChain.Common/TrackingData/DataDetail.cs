namespace TrackingChain.Common.Dto
{
    public class DataDetail
    {
        public virtual string DataValue { get; set; } = default!;
        public virtual long Timestamp { get; set; }
        public virtual long BlockNumber { get; set; }
    }
}
