using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.AggregatorPoolCore.Services;
using TrackingChain.TrackingChainCore.EntityFramework;

namespace TrackingChain.AggregatorPoolCore.UseCases
{
    public class EnqueuerPoolUseCase : IEnqueuerPoolUseCase
    {
        // Fields.
        private readonly IAggregatorService aggregatorService;
        private readonly ILogger<EnqueuerPoolUseCase> logger;
        private readonly IUnitOfWork unitOfWork;

        // Constructors.
        public EnqueuerPoolUseCase(
            IAggregatorService aggregatorService,
            ILogger<EnqueuerPoolUseCase> logger,
            IUnitOfWork unitOfWork)
        {
            this.aggregatorService = aggregatorService;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        // Methods.
        public async Task<int> EnqueueTransactionAsync(int max)
        {
            var triages = await aggregatorService.GetTransactionToEnqueueAsync(max, new List<Guid> { });

            var queues = aggregatorService.Enqueue(triages);

            await aggregatorService.SetToPoolAsync(queues.Select(q => q.TrackingId));

            await unitOfWork.SaveChangesAsync();

            return queues.Count();
        }
    }
}
