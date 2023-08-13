using System;

namespace TrackingChain.TriageWebApplication.ModelView
{
    public class TrackingModelView
    {
        public string Code { get; set; } = default!;
        public string DataValue { get; set; } = default!;
        public DateTime Timestamp { get; set; } = default!;
        public long BlockNumber { get; set; } = default!;
    }
}
