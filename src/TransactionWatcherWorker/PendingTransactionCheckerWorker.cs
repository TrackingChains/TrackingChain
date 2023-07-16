using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionWatcherCore.UseCases;
using TrackingChain.TransactionWatcherWorker.Options;

namespace TransactionWatcherWorker
{
    public class PendingTransactionCheckerWorker : BackgroundService
    {
        private readonly CheckerOptions checkerOptions;
        private readonly ILogger<PendingTransactionCheckerWorker> logger;
        private readonly IServiceProvider serviceProvider;

        public PendingTransactionCheckerWorker(
            IOptions<CheckerOptions> checkerOptions,
            ILogger<PendingTransactionCheckerWorker> logger,
            IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(checkerOptions);

            this.checkerOptions = checkerOptions.Value;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.StartPendingTransactionCheckerWorker();

            // Task creations.
            var tasks = new List<Task>();
            for (int i = 0; i < checkerOptions.Instance; i++)
            {
                tasks.Add(DoAsyncWork(checkerOptions.Accounts[i], stoppingToken));
            }

            await Task.WhenAll(tasks);

            logger.EndPendingTransactionCheckerWorker();
        }

        // Helpers.
        private async Task DoAsyncWork(Guid taskId, CancellationToken stoppingToken)
        {
            logger.StartChildPoolDequeuerTask(taskId);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<PendingTransactionCheckerWorker>();
                var poolDequeuerUseCase = scope.ServiceProvider.GetRequiredService<IPendingTransactionWatcherUseCase>();

                bool dequeued = false;
#pragma warning disable CS0168 // Variable is declared but never used
                try
                {
                    dequeued = await poolDequeuerUseCase.CheckTransactionStatusAsync(checkerOptions.Instance, taskId);
                }
#pragma warning disable CA1031 // 
                catch (Exception ex)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                    logger.LogError(ex, "Errore in DoAsyncWork");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore CA1031 // 
                await Task.Delay(dequeued ? 100 : 1000, stoppingToken);
            }
            logger.EndChildPoolDequeuerTask(taskId);
        }
    }
}