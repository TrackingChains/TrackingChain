using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using System;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace TrackingChain.TriageWebApplication.HostedService
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
            //logger.StartMigratorDbWorker();

            using var scope = serviceProvider.CreateScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<MigratorDBHostedService>();
            var databaseConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

            if (databaseConfig.Value.UseMigrationScript)
            {
                //logger.RunMigrateDbTransactionPool(nameof(ApplicationDbContext));
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync(cancellationToken);
            }

            //logger.EndMigratorDbWorker();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
