using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionMonitorWorker
{
    public class WaitingDBHostedService : IHostedService
    {
        private readonly ILogger<WaitingDBHostedService> logger;
        private readonly IServiceProvider serviceProvider;

        public WaitingDBHostedService(
            ILogger<WaitingDBHostedService> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        _ = await applicationDbContext.Accounts.FirstOrDefaultAsync(cancellationToken);
                        return;
                    }
                }
#pragma warning disable CA1031 //
                catch (Exception ex)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                    logger.LogWarning(ex, "Waiting for database");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                    await Task.Delay(5000, cancellationToken);
                }
#pragma warning restore CA1031 //
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
