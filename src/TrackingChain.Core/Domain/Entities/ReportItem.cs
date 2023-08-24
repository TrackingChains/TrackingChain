using System;
using TrackingChain.Core.Domain.Enums;

namespace TrackingChain.Core.Domain.Entities
{
    public class ReportItem
    {
        // Constructors.
        public ReportItem(
            string description,
            int priority,
            bool reported,
            ReportItemType type,
            Guid transactionId)
        {
            ArgumentException.ThrowIfNullOrEmpty(description);

            this.Created = DateTime.UtcNow;
            this.Description = description;
            this.Priority = priority;
            this.Reported = reported;
            this.Type = type;
            this.TransactionId = transactionId;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ReportItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid Id { get; private set; }
        public DateTime Created { get; private set; }
        public string Description { get; private set; }
        public int Priority { get; private set; }
        public bool Reported { get; private set; }
        public ReportItemType Type { get; private set; }
        public Guid TransactionId { get; private set; }
        public virtual ReportData ReportData { get; private set; }
        public Guid? ReportDataId { get; private set; }

        // Methods.
        public void SetReported(ReportData reportData)
        {
            if (ReportDataId is not null)
            {
                var ex = new InvalidOperationException("SetReported can be call only for item with ReportData NULL");
                ex.Data.Add("Id", Id);
                throw ex;
            }
            ReportData = reportData;
            Reported = true;
        }
    }
}
