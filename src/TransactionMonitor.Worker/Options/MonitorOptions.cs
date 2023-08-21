namespace TrackingChain.TransactionRecoveryWorker.Options
{
    public class MonitorOptions
    {
        public int FailedReTryTimes { get; set; }
        public int GetMaxCompletedTransaction { get; set; }
        public int GetMaxFailedTransaction { get; set; }
        public int GetMaxUnlockTimeout { get; set; }
        public int UnlockUncompletedAfterSeconds { get; set; }
    }
}
