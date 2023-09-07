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
    public class AlertWorker : BackgroundService
    {
        private readonly ILogger<AlertWorker> logger;
        private readonly MonitorOptions monitorOptions;
        private readonly IServiceProvider serviceProvider;

        public AlertWorker(
            ILogger<AlertWorker> logger,
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
            logger.StartAlertWorker();
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<TransactionLockedWorker>();
                var alertUseCase = scope.ServiceProvider.GetRequiredService<IAlertUseCase>();

                var itemDeleted = 0;
                try
                {
                    /*await alertUseCase.RunAsync(
                        TimeSpan.FromMinutes(monitorOptions.IntervalMinutesBetweenTransactionCancelledReport),
                        TimeSpan.FromMinutes(monitorOptions.IntervalMinutesBetweenTransactionErrorReport));*/
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
                {
                    itemDeleted = 0;
                    logger.AlertWorkerError(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
                await Task.Delay(
                    itemDeleted < monitorOptions.GetMaxCompletedTransaction ?
                        (int)TimeSpan.FromMinutes(60).TotalMilliseconds :
                        (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                    stoppingToken);
                

                await Task.Delay(1500, stoppingToken);
            }
            logger.EndAlertWorker();
        }
    }
}
