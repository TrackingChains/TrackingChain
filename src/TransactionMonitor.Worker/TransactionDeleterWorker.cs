using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionMonitorCore.UseCases;
using TrackingChain.TransactionRecoveryWorker.Options;

namespace TrackingChain.TransactionMonitorWorker
{
    public class TransactionDeleterWorker : BackgroundService
    {
        private readonly ILogger<TransactionDeleterWorker> logger;
        private readonly MonitorOptions monitorOptions;
        private readonly IServiceProvider serviceProvider;

        public TransactionDeleterWorker(
            ILogger<TransactionDeleterWorker> logger,
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
            logger.StartTransactionDeleterWorker();
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<TransactionLockedWorker>();
                var transactionLockedUseCase = scope.ServiceProvider.GetRequiredService<ITransactionLockedUseCase>();

                await transactionLockedUseCase.ReProcessAsync(monitorOptions.GetMaxFailedTransaction, monitorOptions.FailedReTryTimes);

                await Task.Delay(1500, stoppingToken);
            }
            logger.EndTransactionDeleterWorker();
        }
    }
}
