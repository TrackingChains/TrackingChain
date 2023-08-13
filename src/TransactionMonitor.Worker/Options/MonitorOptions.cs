namespace TrackingChain.TransactionRecoveryWorker.Options
{
    public class MonitorOptions
    {
        public int MaxUnlockTimeout { get; set; }
        public int UnlockUncompletedAfterSeconds { get; set; }
    }
}
