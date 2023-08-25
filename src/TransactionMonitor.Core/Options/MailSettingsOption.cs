namespace TrackingChain.TransactionMonitorCore.Options
{
    public class MailSettingsOption
    {
        public string Mail { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Host { get; set; } = default!;
        public int Port { get; set; }
    }
}
