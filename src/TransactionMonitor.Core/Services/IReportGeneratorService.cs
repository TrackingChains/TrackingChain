using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public interface IReportGeneratorService
    {
        Task<string> GenerateTxCancelReportAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems);
        Task<string> GenerateTxFailedReportAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems);
    }
}
