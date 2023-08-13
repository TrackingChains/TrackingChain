using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionMonitorCore.UseCases;

namespace TrackingChain.TransactionMonitorWorker
{
    public class TransactionFailedRecoveryWorker : BackgroundService
    {
        private readonly ILogger<TransactionFailedRecoveryWorker> logger;
        private readonly IServiceProvider serviceProvider;

        public TransactionFailedRecoveryWorker(
            ILogger<TransactionFailedRecoveryWorker> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.StartPendingTransactionCheckerWorker();
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<TransactionFailedRecoveryWorker>();
                var aggregatorService = scope.ServiceProvider.GetRequiredService<ITransactionFailedUseCase>();

                await aggregatorService.ReProcessAsync(100);

                await Task.Delay(1500, stoppingToken);
            }
            logger.EndPendingTransactionCheckerWorker();
        }
    }
}
