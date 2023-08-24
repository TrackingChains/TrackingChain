using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Enums;
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
        public async Task<TrackingStatusStatisticModelView> GetTrackingStatusStatisticAsync()
        {
            // Items.
            var transactionStatistics = await dbContext.TransactionRegistries
                .GroupBy(tr => tr.TransactionStep)
                .Select(group => new
                {
                    TransactionStep = group.Key,
                    Count = group.Count(),
                    ErrorCount = group.Count(tr =>
                        tr.TransactionStep == TransactionStep.Completed &&
                        tr.Status == RegistryStatus.Error)
                })
                .ToListAsync();

            // Timings.
            var averageTriage = await dbContext.TransactionRegistries
               .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                            tr.Status == RegistryStatus.SuccessfullyCompleted)
               .Select(tr => EF.Functions.DateDiffMillisecond(tr.TriageDate, tr.PoolDate))
               .AverageAsync();
            var averagePool = await dbContext.TransactionRegistries
               .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                            tr.Status == RegistryStatus.SuccessfullyCompleted)
               .Select(tr => EF.Functions.DateDiffMillisecond(tr.PoolDate, tr.PendingDate))
               .AverageAsync();
            var averagePending = await dbContext.TransactionRegistries
               .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                            tr.Status == RegistryStatus.SuccessfullyCompleted)
               .Select(tr => EF.Functions.DateDiffMillisecond(tr.PendingDate, tr.RegistryDate))
               .AverageAsync();
            var averageCompleted = await dbContext.TransactionRegistries
               .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                            tr.Status == RegistryStatus.SuccessfullyCompleted)
               .Select(tr => EF.Functions.DateDiffMillisecond(tr.TriageDate, tr.RegistryDate))
               .AverageAsync();
            var minCompletedTime = await dbContext.TransactionRegistries
               .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                            tr.Status == RegistryStatus.SuccessfullyCompleted &&
                            tr.TriageDate.AddMonths(1) > DateTime.UtcNow)
               .Select(tr => EF.Functions.DateDiffMillisecond(tr.TriageDate, tr.RegistryDate))
               .MinAsync();
            var maxCompletedTime = await dbContext.TransactionRegistries
               .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                            tr.Status == RegistryStatus.SuccessfullyCompleted &&
                            tr.TriageDate.AddMonths(1) > DateTime.UtcNow)
               .Select(tr => EF.Functions.DateDiffMillisecond(tr.TriageDate, tr.RegistryDate))
               .MaxAsync();


            return new TrackingStatusStatisticModelView
            {
                Error = transactionStatistics.Sum(stat => stat.ErrorCount),
                Pending = transactionStatistics
                    .FirstOrDefault(stat => stat.TransactionStep == TransactionStep.Pending)?.Count ?? 0,
                Pool = transactionStatistics
                    .FirstOrDefault(stat => stat.TransactionStep == TransactionStep.Pool)?.Count ?? 0,
                Triage = transactionStatistics
                    .FirstOrDefault(stat => stat.TransactionStep == TransactionStep.Triage)?.Count ?? 0,
                Successful = transactionStatistics
                    .Where(stat => stat.TransactionStep == TransactionStep.Completed)
                    .Sum(stat => stat.Count - stat.ErrorCount),
                TimingAvgTriageToPool = (int)averageTriage,
                TimingAvgPoolToPending = (int)averagePool,
                TimingAvgPendingToCompleted = (int)averagePending,
                TimingAvgTriageToCompleted = (int)averageCompleted,
                TimingMinAvgLastMonthTriageToCompleted = (int)minCompletedTime,
                TimingMaxAvgLastMonthTriageToCompleted = (int)maxCompletedTime,
            };
        }

        public async Task<TrackingModelView?> GetTrackingAsync(Guid trackingGuid)
        {
            var transactionRegistry = await dbContext.TransactionRegistries
                .Where(tr => tr.TrackingId == trackingGuid)
                .FirstOrDefaultAsync();

            if (transactionRegistry is null)
                return null;

            return TrackingModelView.FromEntity(transactionRegistry);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingHistoryAsync(Guid trackingGuid)
        {
            var transactionRegistry = await dbContext.TransactionRegistries
                .Where(tr => tr.TrackingId == trackingGuid)
                .FirstOrDefaultAsync();

            if (transactionRegistry is null)
                return Array.Empty<TrackingModelView>();

            return await GetTrackingHistoryAsync(
                transactionRegistry.Code,
                transactionRegistry.SmartContractId);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingHistoryAsync(
            string code,
            long smartContractId)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.SmartContractId == smartContractId &&
                             tr.Code == code)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingFailedsAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                             (tr.Status == RegistryStatus.Error ||
                              tr.Status == RegistryStatus.CanceledDueToError))
                .Skip(size * (page - 1))
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingPendingsAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Pending)
                .Skip(size * (page - 1))
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingPoolsAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Pool)
                .Skip(size * (page - 1))
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
                .Where(tr => tr.TransactionStep == TransactionStep.Completed &&
                             tr.Status == RegistryStatus.SuccessfullyCompleted)
                .Skip(size * (page - 1))
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }

        public async Task<IEnumerable<TrackingModelView>> GetTrackingTriagesAsync(int size, int page)
        {
            var transactionRegistries = await dbContext.TransactionRegistries
                .Where(tr => tr.TransactionStep == TransactionStep.Triage)
                .Skip(size * (page - 1))
                .Take(size)
                .OrderBy(tr => tr.ReceivedDate)
                .ToListAsync();

            return transactionRegistries.Select(TrackingModelView.FromEntity);
        }
    }
}
