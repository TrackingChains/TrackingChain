using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public class AlertUseCase : IAlertUseCase
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<AlertUseCase> logger;

        // Constructors.
        public AlertUseCase(
            ApplicationDbContext applicationDbContext,
            ILogger<AlertUseCase> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public async Task<bool> RunAsync(int max)
        {
            //TODO dbSet for Alert

            return await applicationDbContext.SaveChangesAsync() > 0;
        }
    }
}
