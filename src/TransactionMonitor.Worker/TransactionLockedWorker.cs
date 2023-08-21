using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionMonitorCore.UseCases;
using TrackingChain.TransactionRecoveryWorker.Options;

namespace TrackingChain.TransactionMonitorWorker
{
    public class TransactionLockedWorker : BackgroundService
    {
        private readonly ILogger<TransactionLockedWorker> logger;
        private readonly MonitorOptions monitorOptions;
        private readonly IServiceProvider serviceProvider;

        public TransactionLockedWorker(
            ILogger<TransactionLockedWorker> logger,
            IOptions<MonitorOptions> monitorOptions,
            IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(monitorOptions);

            this.logger = logger;
            this.monitorOptions = monitorOptions.Value;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.StartTransactionLockedWorker();
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<TransactionLockedWorker>();
                var transactionLockedUseCase = scope.ServiceProvider.GetRequiredService<ITransactionLockedUseCase>();

                var itemDeleted = 0;
                try
                {
                    await transactionLockedUseCase.ReProcessAsync(monitorOptions.GetMaxUnlockTimeout, monitorOptions.UnlockUncompletedAfterSeconds);
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
                {
                    itemDeleted = 0;
                    logger.TransactionLockedWorkerError(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
                await Task.Delay(
                    itemDeleted < monitorOptions.GetMaxUnlockTimeout ?
                        (int)TimeSpan.FromMinutes(1).TotalMilliseconds :
                        (int)TimeSpan.FromSeconds(1).TotalMilliseconds,
                    stoppingToken);
            }
            logger.EndTransactionLockedWorker();
        }
    }
}
