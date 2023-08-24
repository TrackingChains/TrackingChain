using System;
using TrackingChain.Core.Domain.Enums;

namespace TrackingChain.Core.Domain.Entities
{
    public class Report
    {
        // Constructors.
        public Report(
            string description,
            int priority,
            bool reported,
            ReportType type,
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
        protected Report() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid Id { get; private set; }
        public DateTime Created { get; private set; }
        public string Description { get; private set; }
        public int Priority { get; private set; }
        public bool Reported { get; private set; }
        public ReportType Type { get; private set; }
        public Guid TransactionId { get; private set; }

        // Methods.
        void SetReported()
        {
            Reported = true;
        }
    }
}
