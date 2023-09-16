namespace TrackingChain.Core.Domain.Enums
{
    public enum ReportItemType
    {
        Exception = 0,
        LockTimeOut = 1,
        TxAborted = 2,
        TxCancelled = 3,
        TxGenerationFailed = 4,
        TxGenerationInError = 5,
        TxWatchingFailed = 6,
        TxWatchingInError = 7
    }
}
