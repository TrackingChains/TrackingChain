using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TransactionMonitorCore.Options;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public class FileAlertService : IAlertService
    {
        // Fields.
        private readonly IReportGeneratorService reportGeneratorService;
        private readonly ILogger<FileAlertService> logger;
        private readonly FileReportSettingsOption fileReportSettings;

        // Constructors.
        public FileAlertService(
            IReportGeneratorService reportGeneratorService,
            ILogger<FileAlertService> logger,
            IOptions<FileReportSettingsOption> fileReportSettingsOption)
        {
            ArgumentNullException.ThrowIfNull(fileReportSettingsOption);

            this.reportGeneratorService = reportGeneratorService;
            this.logger = logger;
            this.fileReportSettings = fileReportSettingsOption.Value;
        }

        // Methods.
        public async Task<bool> SendReportAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems)
        {
            ArgumentNullException.ThrowIfNull(reportData);

            switch (reportData.Type)
            {
                case ReportDataType.TxCancel:
                    await GenerateTransactionCancelAsync(reportData, reportItems);
                    break;
                case ReportDataType.TxError:
                    await GenerateTransactionErrorAsync(reportData, reportItems);
                    break;
                default:
                    var ex = new NotImplementedException("SendReportAsync not implemented Type");
                    ex.Data.Add("Type", reportData.Type);
                    throw ex;
            }

            return true;
        }

        // Helpers.
        private async Task GenerateTransactionCancelAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems) =>
            await SendFileAsync(
                reportData, 
                fileReportSettings.TxCancelReportOutputPath, 
                await reportGeneratorService.GenerateTxCancelReportAsync(reportData, reportItems));

        private async Task GenerateTransactionErrorAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems) =>
            await SendFileAsync(
                reportData, 
                fileReportSettings.TxFailedReportOutputPath, 
                await reportGeneratorService.GenerateTxFailedReportAsync(reportData, reportItems));

        private async Task SendFileAsync(
            ReportData reportData,
            string path, 
            string content)
        {
            if (!Directory.Exists(path))
            {
                var ex = new InvalidOperationException("SendFile with Invalid path");
                ex.Data.Add("Path", path);
                throw ex;
            }

            var filePath = Path.Combine(path, $"Report_{reportData.Type}_{reportData.Created.ToShortDateString().Replace("/", "-", StringComparison.InvariantCulture)}_{reportData.Created.ToShortTimeString().Replace(":", "-", StringComparison.InvariantCulture)}.html");
            if (File.Exists(filePath))
                File.Delete(filePath);

            await File.WriteAllTextAsync(filePath, content);
        }

    }
}
