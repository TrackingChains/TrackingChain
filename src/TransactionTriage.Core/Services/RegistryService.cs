using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionTriageCore.Services
{
    public class RegistryService : IRegistryService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<RegistryService> logger;

        // Constructors.
        public RegistryService(
            ApplicationDbContext applicationDbContext,
            ILogger<RegistryService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public async Task<SmartContract> GetSmartContractAsync(long smartContractId)
        {
            var smartContract = await applicationDbContext.SmartContracts
                                        .Where(sc => sc.Id == smartContractId)
                                        .FirstOrDefaultAsync();

            if (smartContract is null)
            {
                var ex = new InvalidOperationException("SmartContract not found.");
                ex.Data.Add("SmartContractId", smartContractId);
                throw ex;
            }

            return smartContract;
        }
    }
}
