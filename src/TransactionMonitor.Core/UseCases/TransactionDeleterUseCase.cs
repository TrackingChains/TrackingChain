using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

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
        public async Task<bool> RunAsync(int max)
        {
            var triages = applicationDbContext.TransactionTriages
                .Where(tt => tt.Completed)
                .ToListAsync();
            applicationDbContext.RemoveRange(triages);

            var pools = applicationDbContext.TransactionPools
                .Where(tt => tt.Completed)
                .ToListAsync();
            applicationDbContext.RemoveRange(pools);

            var pendings = applicationDbContext.TransactionPendings
                .Where(tt => tt.Completed)
                .ToListAsync();
            applicationDbContext.RemoveRange(pendings);

            return await applicationDbContext.SaveChangesAsync() > 0;
        }
    }
}
