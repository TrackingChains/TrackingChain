using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.DefaulValues;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TrackingChainCore.Options;

namespace TrackingChain.TriageAPI.HostedService
{
    public class MigratorDBHostedService : IHostedService
    {
        private readonly ILogger<MigratorDBHostedService> logger;
        private readonly IServiceProvider serviceProvider;

        public MigratorDBHostedService(
            ILogger<MigratorDBHostedService> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.StartMigratorDbWorker();

            using (var scope = serviceProvider.CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<MigratorDBHostedService>();
                var databaseConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

                if (databaseConfig.Value.UseMigrationScript)
                {
                    logger.RunMigrateDbTransactionPool(nameof(ApplicationDbContext));
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    await dbContext.Database.MigrateAsync(cancellationToken);
                    await SeedAsync(dbContext);
                }
            }

            logger.EndMigratorDbWorker();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // Helpers.
        private async Task SeedAsync(ApplicationDbContext applicationDbContext)
        {
            var itemToSeed = await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionErrorTemplate);
            if (itemToSeed is null)
                applicationDbContext.Add(new ReportSetting(ReportSetting.TransactionErrorTemplate, ReportDefaultValue.TransactionErrorTemplate));

            itemToSeed = await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionErrorTitle);
            if (itemToSeed is null)
                applicationDbContext.Add(new ReportSetting(ReportSetting.TransactionErrorTitle, ReportDefaultValue.TransactionErrorTitle));

            itemToSeed = await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionErrorMail);
            if (itemToSeed is null)
                applicationDbContext.Add(new ReportSetting(ReportSetting.TransactionErrorMail, ""));

            itemToSeed = await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionCancelledTemplate);
            if (itemToSeed is null)
                applicationDbContext.Add(new ReportSetting(ReportSetting.TransactionCancelledTemplate, ReportDefaultValue.TransactionCancelledTemplate));

            itemToSeed = await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionCancelledTitle);
            if (itemToSeed is null)
                applicationDbContext.Add(new ReportSetting(ReportSetting.TransactionCancelledTitle, ReportDefaultValue.TransactionCancelledTitle));

            itemToSeed = await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionCancelledMail);
            if (itemToSeed is null)
                applicationDbContext.Add(new ReportSetting(ReportSetting.TransactionCancelledMail, ""));

            await applicationDbContext.SaveChangesAsync();
        }
    }
}
