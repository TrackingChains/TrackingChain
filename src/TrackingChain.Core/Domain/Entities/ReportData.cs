using System;
using TrackingChain.Core.Domain.Enums;

namespace TrackingChain.Core.Domain.Entities
{
    public class ReportData
    {
        // Constructors.
        public ReportData(ReportDataType type) : 
            this($"[{DateTime.UtcNow.ToShortDateString()}] Report {type}" , type) { }
        public ReportData(
            string description,
            ReportDataType type)
        {
            ArgumentException.ThrowIfNullOrEmpty(description);

            this.Created = DateTime.UtcNow;
            this.Description = description;
            this.Type = type;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ReportData() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid Id { get; private set; }
        public DateTime Created { get; private set; }
        public string Description { get; private set; }
        public bool Sent { get; private set; }
        public ReportDataType Type { get; private set; }

        // Methods.
        public void SetSent()
        {
            Sent = true;
        }
    }
}