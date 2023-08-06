using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionTriageCore.ModelViews;

namespace TrackingChain.TransactionTriageCore.UseCases
{
    public class AnalyticUseCase : IAnalyticUseCase
    {
        // Fields.
        private readonly ILogger<AnalyticUseCase> logger;
        private readonly ApplicationDbContext dbContext;

        // Constructors.
        public AnalyticUseCase(
            ILogger<AnalyticUseCase> logger,
            ApplicationDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }


        // Methods.
        public async Task<TrackingModelView?> GetTrackingAsync(Guid trackingGuid)
        {
            var transactionRegistry = await dbContext.TransactionRegistries
                .Where(tr => tr.TrackingId == trackingGuid)
                .FirstOrDefaultAsync();

            if (transactionRegistry is null)
                return null;

            return TrackingModelView.FromEntity(transactionRegistry);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingFailedsAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.ReceiptSuccessful.HasValue &&
                             !tr.ReceiptSuccessful.Value)
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingPendingsAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Pending)
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingPoolsAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Pool)
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<TrackingStatusModelView?> GetTrackingStatusAsync(Guid trackingGuid)
        {
            var transactionRegistry = await dbContext.TransactionRegistries
                .Where(tr => tr.TrackingId == trackingGuid)
                .FirstOrDefaultAsync();

            if (transactionRegistry is null)
                return null;

            return TrackingStatusModelView.FromEntity(transactionRegistry);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingSuccessfullyAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.ReceiptSuccessful.HasValue &&
                             tr.ReceiptSuccessful.Value)
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingTriagesAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Triage)
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public Task<IEnumerable<TrackingModelView>> SearchTrackingAsync(Guid profileGroup, string Code)
        {
            throw new NotImplementedException();
        }
    }
}
