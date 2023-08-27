using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TransactionMonitorCore.ModelView;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        public async Task<string> GenerateTxCancelReportAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems)
        {
            var str = new StreamReader("ReportTemplates\\TxCancelModelView.html");
            string bodyTemplate = await str.ReadToEndAsync();
            str.Close();

            return GenerateHtmlContent(bodyTemplate, reportItems.Select(ri => new TxCancelModelView(ri)));
        }

        public async Task<string> GenerateTxFailedReportAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems)
        {
            var str = new StreamReader("ReportTemplates\\TxFailedModelView.html");
            string bodyTemplate = await str.ReadToEndAsync();
            str.Close();

            return GenerateHtmlContent(bodyTemplate, reportItems.Select(ri => new TxCancelModelView(ri)));
        }

        // Helpers.
        private string GenerateHtmlContent(
            string bodyTemplate,
            IEnumerable<TxCancelModelView> tableRows)
        {
            var sb = new StringBuilder();

            foreach (var row in tableRows)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(CultureInfo.InvariantCulture, $"<td>{row.TrackingId}</td>");
                sb.AppendLine(CultureInfo.InvariantCulture, $"<td>{row.DateTime}</td>");
                sb.AppendLine(CultureInfo.InvariantCulture, $"<td>{row.Description}</td>");
                sb.AppendLine("</tr>");
            }

            return bodyTemplate.Replace("[TR_PLACEHOLDER]", sb.ToString(), StringComparison.InvariantCulture);
        }
    }
}
