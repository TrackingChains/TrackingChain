using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionGeneratorCore.UseCases;
using TrackingChain.TransactionGeneratorWorker.Options;

namespace TrackingChain.TransactionGeneratorWorker
{
    public class PoolDequeuerWorker : BackgroundService
    {
        private readonly DequeuerOptions dequeuerOptions;
        private readonly ILogger<PoolDequeuerWorker> logger;
        private readonly IServiceProvider serviceProvider;

        public PoolDequeuerWorker(
            IOptions<DequeuerOptions> dequeuerOptions,
            ILogger<PoolDequeuerWorker> logger,
            IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(dequeuerOptions);

            this.dequeuerOptions = dequeuerOptions.Value;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.StartPoolDequeuerWorker();

            if (dequeuerOptions.Instance != dequeuerOptions.Accounts.Count)
            {
                //TODO controllare duplicati
            }

            // Task creations.
            var tasks = new List<Task>();
            for (int i = 0; i < dequeuerOptions.Instance; i++)
            {
                tasks.Add(DoAsyncWork(dequeuerOptions.Accounts[i], stoppingToken));
            }

            await Task.WhenAll(tasks);

            logger.EndPoolDequeuerWorker();
        }

        // Helpers.
        private async Task DoAsyncWork(Guid taskId, CancellationToken stoppingToken)
        {
            logger.StartChildPoolDequeuerTask(taskId);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<PoolDequeuerWorker>();
                var poolDequeuerUseCase = scope.ServiceProvider.GetRequiredService<IPoolDequeuerUseCase>();

                bool dequeued = false;
#pragma warning disable CS0168 // Variable is declared but never used
                try
                {
                    dequeued = await poolDequeuerUseCase.DequeueTransactionAsync(dequeuerOptions.Instance, taskId);
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
                await Task.Delay(dequeued ? 500 : 1000, stoppingToken);
            }
            logger.EndChildPoolDequeuerTask(taskId);
        }
    }
}