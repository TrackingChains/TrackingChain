using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionMonitorCore.Services;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public class AlertUseCase : IAlertUseCase
    {
        // Fields.
        private readonly IAlertService alertService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<AlertUseCase> logger;
        private readonly ITransactionMonitorService transactionMonitorService;

        // Constructors.
        public AlertUseCase(
            IAlertService alertService,
            ApplicationDbContext applicationDbContext,
            ILogger<AlertUseCase> logger,
            ITransactionMonitorService transactionMonitorService)
        {
            this.alertService = alertService;
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
            this.transactionMonitorService = transactionMonitorService;
        }

        // Methods.
        public async Task<bool> RunAsync(
            TimeSpan intervalBetweenTransactionCancelledReport,
            TimeSpan intervalBetweenTransactionErrorReport)
        {
            var anyReport = false;
            if (await transactionMonitorService.IsInTimeForGenerateTransactionCancelledReportAsync(intervalBetweenTransactionCancelledReport))
            {
                var reportItems = await transactionMonitorService.GenerateTransactionCancelledReportItemAsync();

                if (reportItems.Any())
                {
                    var reportData = new ReportData($"Report TxCancel {DateTime.UtcNow.ToShortDateString()}", ReportDataType.TxCancel);
                    applicationDbContext.Add(reportData);

                    foreach (var item in reportItems)
                        item.SetReported(reportData);

                    anyReport = true;
                    await applicationDbContext.SaveChangesAsync();
                    
                    await alertService.SendReportAsync(reportData, reportItems);
                    reportData.SetSent();

                    await applicationDbContext.SaveChangesAsync();
                }
            }

            if (await transactionMonitorService.IsInTimeForGenerateTransactionErrorReportAsync(intervalBetweenTransactionErrorReport))
            {
                var reportItems = await transactionMonitorService.GenerateTransactionErrorReportItemAsync();
                if (reportItems.Any())
                {
                    var reportData = new ReportData($"Report TxError {DateTime.UtcNow.ToShortDateString()}", ReportDataType.TxError);
                    applicationDbContext.Add(reportData);

                    foreach (var item in reportItems)
                        item.SetReported(reportData);

                    anyReport = true;
                    await applicationDbContext.SaveChangesAsync();

                    await alertService.SendReportAsync(reportData, reportItems);
                    reportData.SetSent();

                    await applicationDbContext.SaveChangesAsync();
                }
            }

            await applicationDbContext.SaveChangesAsync();

            return anyReport;
        }
    }
}
