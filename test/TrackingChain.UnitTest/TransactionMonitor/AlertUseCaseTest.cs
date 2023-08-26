using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorCore.Services;
using TrackingChain.TransactionMonitorCore.UseCases;
using Xunit;

namespace TrackingChain.UnitTest.TransactionMonitor
{
#pragma warning disable CA1001 // Not need in unit test
    public class AlertUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<IAlertService> mockAlertService;
        private readonly Mock<ITransactionMonitorService> mockTransactionMonitorService;

        public AlertUseCaseTest()
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

            //services
            mockAlertService = new Mock<IAlertService>();
            mockTransactionMonitorService = new Mock<ITransactionMonitorService>();
        }

        [Fact]
        public async Task TransactionDeleterShouldRemoveTriageCompletedAsync()
        {
            //Arrange
            var mockAlertService = new[] { new Mock<IAlertService>(), new Mock<IAlertService>() };

            mockTransactionMonitorService
                .Setup(m => m.IsInTimeForGenerateTransactionErrorReportAsync(It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult(true));
            mockTransactionMonitorService
                .Setup(m => m.IsInTimeForGenerateTransactionCancelledReportAsync(It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult(true));

            dbContext.ReportItems.Add(new ReportItem("1", 1, false, ReportItemType.TxGenerationInError, Guid.NewGuid()));
            dbContext.ReportItems.Add(new ReportItem("2", 2, false, ReportItemType.TxGenerationInError, Guid.NewGuid()));
            dbContext.ReportItems.Add(new ReportItem("3", 3, false, ReportItemType.TxGenerationInError, Guid.NewGuid()));
            dbContext.ReportItems.Add(new ReportItem("4", 4, false, ReportItemType.TxGenerationFailed, Guid.NewGuid()));
            dbContext.ReportItems.Add(new ReportItem("5", 5, false, ReportItemType.TxGenerationFailed, Guid.NewGuid()));
            dbContext.ReportItems.Add(new ReportItem("6", 6, false, ReportItemType.LockTimeOut, Guid.NewGuid()));
            dbContext.ReportItems.Add(new ReportItem("7", 7, false, ReportItemType.TxCancelled, Guid.NewGuid()));
            await dbContext.SaveChangesAsync();

            var itemErrors = await dbContext.ReportItems.Where(ri => ri.Type == ReportItemType.TxGenerationInError).ToListAsync();
            mockTransactionMonitorService
                .Setup(m => m.GenerateTransactionErrorReportItemAsync())
                .Returns(Task.FromResult((IEnumerable<ReportItem>)itemErrors));

            var itemFaileds = await dbContext.ReportItems.Where(ri => ri.Type == ReportItemType.TxGenerationFailed).ToListAsync();
            mockTransactionMonitorService
                .Setup(m => m.GenerateTransactionCancelledReportItemAsync())
                .Returns(Task.FromResult((IEnumerable<ReportItem>)itemFaileds));

            var alertUseCase = new AlertUseCase(
                mockAlertService.Select(mm => mm.Object),
                dbContext,
                Mock.Of<ILogger<AlertUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            var deletedResult = await alertUseCase.RunAsync(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));


            //Assert
            itemErrors = await dbContext.ReportItems.Where(ri => ri.Type == ReportItemType.TxGenerationInError).ToListAsync();
            Assert.True(itemErrors.All(ri => ri.Reported));

            itemFaileds = await dbContext.ReportItems.Where(ri => ri.Type == ReportItemType.TxGenerationFailed).ToListAsync();
            Assert.True(itemFaileds.All(ri => ri.Reported));

            var reportDatas = await dbContext.ReportData.ToListAsync();
            Assert.Equal(2, reportDatas.Count);
        }
    }
}
