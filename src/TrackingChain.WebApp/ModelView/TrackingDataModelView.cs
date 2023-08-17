using System;

namespace TrackingChain.TriageWebApplication.ModelView
{
    public class TrackingDataModelView
    {
        public string Code { get; set; } = default!;
        public string DataValue { get; set; } = default!;
        public DateTime Timestamp { get; set; } = default!;
        public string BlockNumber { get; set; } = default!;
    }
}
