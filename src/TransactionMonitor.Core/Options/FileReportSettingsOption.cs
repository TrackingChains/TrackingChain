namespace TrackingChain.TransactionMonitorCore.Options
{
    public class FileReportSettingsOption
    {
        public string TxCancelReportOutputPath { get; set; } = default!;
        public string TxFailedReportOutputPath { get; set; } = default!;
    }
}
