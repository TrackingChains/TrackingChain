using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TransactionWaitingCore.Services;

namespace TrackingChain.TransactionTriageCore.UseCases
{
    public class TrackingEntryUseCase : ITrackingEntryUseCase
    {
        // Fields.
        private readonly ILogger<TrackingEntryUseCase> logger;
        private readonly ITransactionTriageService transactionTriageService;
        private readonly IUnitOfWork unitOfWork;

        // Constructors.
        public TrackingEntryUseCase(
            ILogger<TrackingEntryUseCase> logger,
            ITransactionTriageService transactionTriageService,
            IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.transactionTriageService = transactionTriageService;
            this.unitOfWork = unitOfWork;
        }

        // Methods.
        public async Task<string> AddTransactionAsync(
            string code, 
            string data, 
            string category)
        {
            var triage = await transactionTriageService.AddTransactionAsync(code, category, data);
            transactionTriageService.AddRegistry(triage);

            await unitOfWork.SaveChangesAsync();

            return triage.TrackingIdentify.ToString();
        }
    }
}
