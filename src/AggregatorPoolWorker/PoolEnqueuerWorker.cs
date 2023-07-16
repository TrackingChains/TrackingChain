using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.AggregatorPoolCore.UseCases;
using TrackingChain.TrackingChainCore.Extensions;

namespace TrackingChain.AggregatorPoolWorker
{
    public class PoolEnqueuerWorker : BackgroundService
    {
        private readonly ILogger<PoolEnqueuerWorker> logger;
        private readonly IServiceProvider serviceProvider;

        public PoolEnqueuerWorker(
            ILogger<PoolEnqueuerWorker> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.StartPoolEnqueuerWorker();
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<PoolEnqueuerWorker>();
                var aggregatorService = scope.ServiceProvider.GetRequiredService<IEnqueuerPoolUseCase>();

                await aggregatorService.EnqueueTransactionAsync(100);

                await Task.Delay(1500, stoppingToken);
            }
            logger.EndPoolEnqueuerWorker();
        }
    }
}