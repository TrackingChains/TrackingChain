using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public class TransactionMonitorService : ITransactionMonitorService
    {
        // Const.
        private readonly List<ReportItemType> TransactionErrorTypes = new List<ReportItemType> { ReportItemType.TxGenerationFailed, ReportItemType.TxWatchingFailed, ReportItemType.TxGenerationInError, ReportItemType.TxWatchingInError };
        private readonly List<ReportItemType> TransactionCancelledTypes = new List<ReportItemType> { ReportItemType.TxCancelled };

        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<TransactionMonitorService> logger;

        // Constructors.
        public TransactionMonitorService(
            ApplicationDbContext applicationDbContext,
            ILogger<TransactionMonitorService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Methods.

        public async Task<IEnumerable<ReportItem>> GenerateTransactionCancelledReportItemAsync()
        {
            return await applicationDbContext.ReportItems
                .Where(tp => !tp.Reported &&
                              TransactionCancelledTypes.Contains(tp.Type))
                .OrderBy(tp => tp.Created)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReportItem>> GenerateTransactionErrorReportItemAsync()
        {
            return await applicationDbContext.ReportItems
                .Where(tp => !tp.Reported &&
                              TransactionErrorTypes.Contains(tp.Type))
                .OrderBy(tp => tp.Created)
                .ToListAsync();
        }

        public async Task<bool> IsInTimeForGenerateTransactionCancelledReportAsync(TimeSpan intervalBetweenLastReport) =>
            await IsInTimeAsync(ReportDataType.TxCancel, intervalBetweenLastReport);

        public async Task<bool> IsInTimeForGenerateTransactionErrorReportAsync(TimeSpan intervalBetweenLastReport) =>
            await IsInTimeAsync(ReportDataType.TxError, intervalBetweenLastReport);

        public async Task<IEnumerable<TransactionPending>> GetPendingLockedInTimeoutAsync(
            int max, 
            int timeoutSeconds)
        {
            return await applicationDbContext.TransactionPendings
                .Where(tp => !tp.Completed &&
                             tp.Locked &&
                             tp.LockedDated!.Value.AddSeconds(timeoutSeconds) < DateTime.UtcNow)
                .OrderBy(tp => tp.LockedDated)
                .Take(max)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionPool>> GetPoolLockedInTimeoutAsync(
            int max, 
            int timeoutSeconds)
        {
            return await applicationDbContext.TransactionPools
                .Where(tp => !tp.Completed &&
                             tp.Locked &&
                             tp.LockedDated!.Value.AddSeconds(timeoutSeconds) < DateTime.UtcNow)
                .OrderBy(tp => tp.LockedDated)
                .Take(max)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionRegistry>> GetTransactionWaitingToReProcessAsync(int max)
        {
            return await applicationDbContext.TransactionRegistries
                .Where(tr => tr.Status == RegistryStatus.WaitingToReTry ||
                             tr.Status == RegistryStatus.WaitingToCancel)
                .OrderBy(tp => tp.ReceivedDate)
                .Take(max)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionPending>> GetTransactionPendingAsync(IEnumerable<Guid> ids)
        {
            return await applicationDbContext.TransactionPendings
                .Where(tr => ids.Contains(tr.TrackingId))
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionPool>> GetTransactionPoolAsync(IEnumerable<Guid> ids)
        {
            return await applicationDbContext.TransactionPools
                .Where(tr => ids.Contains(tr.TrackingId))
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionTriage>> GetTransactionTriageAsync(IEnumerable<Guid> ids)
        {
            return await applicationDbContext.TransactionTriages
                .Where(tr => ids.Contains(tr.TrackingIdentify))
                .ToListAsync();
        }

        // Helpers
        private async Task<bool> IsInTimeAsync(
            ReportDataType reportDataType,
            TimeSpan intervalBetweenLastReport)
        {
            var maxDate = await applicationDbContext.ReportData
                .Where(tp => tp.Type == reportDataType)
                .MaxAsync(tr => tr.Created);

            return DateTime.UtcNow > maxDate.Add(intervalBetweenLastReport);
        }
    }
}
