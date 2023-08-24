using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public interface IAlertService
    {
        Task<bool> SendReportAsync(ReportData reportData, IEnumerable<ReportItem> reportItems);
    }
}
