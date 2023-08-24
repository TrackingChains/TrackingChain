namespace TrackingChain.Core.Domain.Enums
{
    public enum ReportType
    {
        Exception = 0,
        LockTimeOut = 1,
        TxGenerationFailed = 2,
        TxWatchingFailed = 3,
        TxGenerationInError = 4,
        TxWatchingInError = 5
    }
}
