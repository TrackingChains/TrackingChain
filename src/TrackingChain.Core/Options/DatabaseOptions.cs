namespace TrackingChain.TrackingChainCore.Options
{
    public class DatabaseOptions
    {
        public string DbType { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
        public bool UseMigrationScript { get; set; }
    }
}
