using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public class TransactionFailedUseCase : ITransactionFailedUseCase
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionFailedUseCase> logger;

        // Constructors.
        public TransactionFailedUseCase(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionFailedUseCase> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.
        public Task ReProcessAsync(int max)
        {
            throw new NotImplementedException();
        }
    }
}
