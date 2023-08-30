using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TransactionMonitorCore.Services;
using Xunit;

namespace TrackingChain.UnitTest.TransactionMonitor
{
#pragma warning disable CA1001 // Not need in unit test
    public class ReportGeneratorServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        public ReportGeneratorServiceTest()
        {
            
        }

        [Fact]
        public async Task GenerateTxFailedReportShouldHaveAllItemsAsync()
        {
            //Arrange
            var reportItems = new List<ReportItem>() {
                new ReportItem("1", 1, false, ReportItemType.TxGenerationInError, Guid.NewGuid()),
                new ReportItem("2", 2, false, ReportItemType.TxGenerationInError, Guid.NewGuid()),
                new ReportItem("3", 3, false, ReportItemType.TxGenerationInError, Guid.NewGuid()),
                new ReportItem("4", 4, false, ReportItemType.TxGenerationFailed, Guid.NewGuid())
            };
            var reportData = new ReportData(ReportDataType.TxError);
            var reportGeneratorService = new ReportGeneratorService();

            //Act
            var output = await reportGeneratorService.GenerateTxFailedReportAsync(reportData, reportItems);


            //Assert
            Assert.Equal(reportItems.Count + 1, output.Select((c, i) => output.Substring(i)).Count(sub => sub.StartsWith("<tr>", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Fact]
        public async Task GenerateTxCancelReportShouldHaveAllItemsAsync()
        {
            //Arrange
            var reportItems = new List<ReportItem>() {
                new ReportItem("1", 1, false, ReportItemType.TxCancelled, Guid.NewGuid()),
                new ReportItem("2", 2, false, ReportItemType.TxCancelled, Guid.NewGuid()),
                new ReportItem("3", 3, false, ReportItemType.TxCancelled, Guid.NewGuid()),
                new ReportItem("4", 4, false, ReportItemType.TxCancelled, Guid.NewGuid())
            };
            var reportData = new ReportData(ReportDataType.TxCancel);
            var reportGeneratorService = new ReportGeneratorService();

            //Act
            var output = await reportGeneratorService.GenerateTxCancelReportAsync(reportData, reportItems);


            //Assert
            Assert.Equal(reportItems.Count + 1, output.Select((c, i) => output.Substring(i)).Count(sub => sub.StartsWith("<tr>", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}

