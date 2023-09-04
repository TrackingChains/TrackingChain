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
            if (checkerOptions.Accounts == null ||
                checkerOptions.Accounts.Any())
            {
                logger.EndPendingTransactionCheckerWorker();
                return;
            }

            // Task creations.
            var tasks = new List<Task>();
            foreach (var account in checkerOptions.Accounts.Distinct())
                tasks.Add(RunSingleAccountAsync(account, stoppingToken));

            await Task.WhenAll(tasks);

            logger.EndPendingTransactionCheckerWorker();
        }

        // Helpers.
        private async Task RunSingleAccountAsync(Guid taskId, CancellationToken stoppingToken)
        {
            logger.StartChildCheckerTask(taskId);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<PendingTransactionCheckerWorker>();
                var poolDequeuerUseCase = scope.ServiceProvider.GetRequiredService<IPendingTransactionWatcherUseCase>();

                Guid dequeued;
                try
                {
                    dequeued = await poolDequeuerUseCase.CheckTransactionStatusAsync(
                        checkerOptions.Accounts.Count, 
                        taskId, 
                        checkerOptions.ReTryAfterSeconds, 
                        checkerOptions.ErrorAfterReTry);
                }
#pragma warning disable CA1031 // We need fot catch all problems.
                catch (Exception ex)
                {
                    dequeued = Guid.Empty;
                    logger.ChildCheckerTaskInError(taskId, ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
                await Task.Delay(dequeued == Guid.Empty ? 1000 : 500, stoppingToken);
            }
            logger.EndChildCheckerTask(taskId);
        }
    }
}