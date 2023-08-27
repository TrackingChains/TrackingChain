namespace TrackingChain.TransactionRecoveryWorker.Options
{
    public class MonitorOptions
    {
        public int FailedReTryTimes { get; set; }
        public int GetMaxAlert { get; set; }
        public int GetMaxCompletedTransaction { get; set; }
        public int GetMaxFailedTransaction { get; set; }
        public int GetMaxUnlockTimeout { get; set; }
        public int IntervalMinutesBetweenTransactionCancelledReport { get; set; }
        public int IntervalMinutesBetweenTransactionErrorReport { get; set; }
        public int UnlockUncompletedGeneratorAfterSeconds { get; set; }
        public int UnlockUncompletedWatcherAfterSeconds { get; set; }
    }
}
