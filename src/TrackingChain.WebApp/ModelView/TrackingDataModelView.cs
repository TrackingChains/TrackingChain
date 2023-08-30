using System;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TriageWebApplication.ModelView
{
    public class TrackingDataModelView
    {
        public Guid TrackingId { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DataValue { get; set; } = default!;
        public DateTime Timestamp { get; set; } = default!;
        public string BlockNumber { get; set; } = default!;
        public bool Selected { get; set; } = default!;
        public TransactionStep TransactionStep { get; set; } = default!;
    }
}
