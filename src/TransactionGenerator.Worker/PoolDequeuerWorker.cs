using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // Task creations.
            var tasks = new List<Task>();

            foreach (var account in dequeuerOptions.Accounts.Distinct())
                tasks.Add(RunSingleAccountAsync(account, stoppingToken));

            await Task.WhenAll(tasks);

            logger.EndPoolDequeuerWorker();
        }

        // Helpers.
        private async Task RunSingleAccountAsync(
            Guid taskId, 
            CancellationToken stoppingToken)
        {
            logger.StartChildPoolDequeuerTask(taskId);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<PoolDequeuerWorker>();
                var poolDequeuerUseCase = scope.ServiceProvider.GetRequiredService<IPoolDequeuerUseCase>();

                bool dequeued = false;
                try
                {
                    dequeued = await poolDequeuerUseCase.DequeueTransactionAsync(
                        dequeuerOptions.Accounts.Count, 
                        taskId,
                        dequeuerOptions.ReTryAfterSeconds,
                        dequeuerOptions.SaveAsErrorAfterSeconds);
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.ChildPoolDequeuerTaskInError(taskId, ex);
                }
                await Task.Delay(dequeued ? 500 : 1000, stoppingToken);
            }
            logger.EndChildPoolDequeuerTask(taskId);
        }
    }
}