using System;
using TrackingChain.Core.Domain.Entities;

namespace TrackingChain.TransactionMonitorCore.ModelView
{
    public class TxFailedModelView
    {
        public TxFailedModelView(ReportItem reportItem)
        {
            ArgumentNullException.ThrowIfNull(reportItem);

            TrackingId = reportItem.TrackingId;
            DateTime = reportItem.Created;
            Description = reportItem.Description;
        }

        public Guid TrackingId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = default!;
    }
}
