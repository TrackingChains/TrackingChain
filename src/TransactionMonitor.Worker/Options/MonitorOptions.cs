namespace TrackingChain.TransactionRecoveryWorker.Options
{
    public class MonitorOptions
    {
        public int FailedReTryTimes { get; set; }
        public int MaxFailedTransaction { get; set; }
        public int MaxUnlockTimeout { get; set; }
        public int UnlockUncompletedAfterSeconds { get; set; }
    }
}
