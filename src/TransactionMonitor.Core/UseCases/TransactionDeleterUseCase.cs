using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public class TransactionDeleterUseCase : ITransactionDeleterUseCase
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionDeleterUseCase> logger;

        // Constructors.
        public TransactionDeleterUseCase(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionDeleterUseCase> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public async Task<int> RunAsync(int max)
        {
            logger.StartTransactionDeleterUseCase(max);

            var triages = await applicationDbContext.TransactionTriages
                .Where(tt => tt.Completed)
                .Take(max)
                .ToListAsync();
            applicationDbContext.RemoveRange(triages);

            var pools = await applicationDbContext.TransactionPools
                .Where(tt => tt.Completed)
                .Take(max)
                .ToListAsync();
            applicationDbContext.RemoveRange(pools);

            var pendings = await applicationDbContext.TransactionPendings
                .Where(tt => tt.Completed)
                .Take(max)
                .ToListAsync();
            applicationDbContext.RemoveRange(pendings);

            await applicationDbContext.SaveChangesAsync();

            var countMax = new int[] { triages.Count, pools.Count, pendings.Count }.Max();
            logger.EndTransactionDeleterUseCase(countMax);
            return countMax;
        }
    }
}
