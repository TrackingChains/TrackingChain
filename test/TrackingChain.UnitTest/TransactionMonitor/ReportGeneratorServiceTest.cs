using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.DefaulValues;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorCore.Services;
using Xunit;

namespace TrackingChain.UnitTest.TransactionMonitor
{
#pragma warning disable CA1001 // Not need in unit test
    public class ReportGeneratorServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;

        public ReportGeneratorServiceTest()
        {
            //unit of work
            var databaseOptions = new DatabaseOptions()
            {
                ConnectionString = "",
                DbType = "InMemory",
                UseMigrationScript = false
            };
            var mock = new Mock<IOptionsSnapshot<DatabaseOptions>>();
            mock.Setup(m => m.Value).Returns(databaseOptions);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            dbContext = new ApplicationDbContext(options, mock.Object);

            dbContext.Add(new ReportSetting(ReportSetting.TransactionErrorTemplate, ReportDefaultValue.TransactionErrorTemplate));
            dbContext.Add(new ReportSetting(ReportSetting.TransactionErrorMail, ""));
            dbContext.Add(new ReportSetting(ReportSetting.TransactionCancelledTemplate, ReportDefaultValue.TransactionCancelledTemplate));
            dbContext.Add(new ReportSetting(ReportSetting.TransactionCancelledMail, ""));
            dbContext.SaveChanges();
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
            var reportGeneratorService = new ReportGeneratorService(
                dbContext,
                Mock.Of<ILogger<ReportGeneratorService>>());


            //Act
            var output = await reportGeneratorService.GenerateTxFailedReportAsync(reportData, reportItems);


            //Assert
            Assert.Equal(reportItems.Count + 1, output.Select((c, i) => output[i..]).Count(sub => sub.StartsWith("<tr>", StringComparison.InvariantCultureIgnoreCase)));
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
            var reportGeneratorService = new ReportGeneratorService(
                dbContext,
                Mock.Of<ILogger<ReportGeneratorService>>());


            //Act
            var output = await reportGeneratorService.GenerateTxCancelReportAsync(reportData, reportItems);


            //Assert
            Assert.Equal(reportItems.Count + 1, output.Select((c, i) => output[i..]).Count(sub => sub.StartsWith("<tr>", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}

