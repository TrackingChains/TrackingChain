using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TrackingChainCore.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<UnitOfWork> logger;

        // Constructors.
        public UnitOfWork(
            ILogger<UnitOfWork> logger,
            ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public async Task SaveChangesAsync()
        {
            await applicationDbContext.SaveChangesAsync();
        }
    }
}
