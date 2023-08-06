using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.Extensions;
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
        public async Task<Guid> AddTransactionAsync(
            string authority,
            string code,
            string data,
            string category)
        {
            var triage = await transactionTriageService.AddTransactionAsync(authority, code, category, data);
            transactionTriageService.AddRegistry(triage);

            await unitOfWork.SaveChangesAsync();

            logger.TrackingEntry(
                triage.Code, 
                triage.DataValue, 
                triage.SmartContractAddress, 
                triage.ProfileGroupId);
            return triage.TrackingIdentify;
        }
    }
}
