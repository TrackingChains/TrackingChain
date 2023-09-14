using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Options;

namespace TrackingChain.TrackingChainCore.EntityFramework.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DatabaseOptions databaseOption;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptionsSnapshot<DatabaseOptions> databaseOption) : base(options)
        {
            if (databaseOption is null)
                throw new ArgumentNullException(nameof(databaseOption));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            this.databaseOption = databaseOption.Value;
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountProfileGroup> AccountProfileGroup { get; set; } = null!;
        public virtual DbSet<SmartContract> SmartContracts { get; set; } = null!;
        public virtual DbSet<ProfileGroup> ProfileGroups { get; set; } = null!;
        public virtual DbSet<TransactionRegistry> TransactionRegistries { get; set; } = null!;
        public virtual DbSet<TransactionPending> TransactionPendings { get; set; } = null!;
        public virtual DbSet<TransactionTriage> TransactionTriages { get; set; } = null!;
        public virtual DbSet<TransactionPool> TransactionPools { get; set; } = null!;
        public virtual DbSet<ReportData> ReportData { get; set; } = null!;
        public virtual DbSet<ReportItem> ReportItems { get; set; } = null!;
        public virtual DbSet<ReportSetting> ReportSettings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder is not null &&
                !optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(databaseOption.ConnectionString)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            //TODO WARNING this code load all configure, with multiple context need to take only DatabaseContext config class
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
