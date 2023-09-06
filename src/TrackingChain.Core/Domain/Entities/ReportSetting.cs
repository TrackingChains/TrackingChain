using System;

namespace TrackingChain.Core.Domain.Entities
{
    public class ReportSetting
    {
        public const string TransactionErrorMail = "TransactionErrorMail";
        public const string TransactionErrorTemplate = "TransactionErrorTemplate";
        public const string TransactionCancelledMail = "TransactionCancelledMail";
        public const string TransactionCancelledTemplate = "TransactionCancelledTemplate";

        // Constructors.
        public ReportSetting(
            string key,
            string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            this.Key = key;
            this.Value = value;
        }
        protected ReportSetting() { }

        // Properties.
        public string Key { get; private set; } = default!;
        public string? Value { get; set; }
    }
}
