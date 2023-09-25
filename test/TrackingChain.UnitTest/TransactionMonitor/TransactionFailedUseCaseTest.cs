using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorCore.Services;
using TrackingChain.TransactionMonitorCore.UseCases;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.TransactionMonitor
{
#pragma warning disable CA1001 // Not need in unit test
    public class TransactionFailedUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<ITransactionMonitorService> mockTransactionMonitorService;

        public TransactionFailedUseCaseTest()
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

            //transaction generator service
            mockTransactionMonitorService = new Mock<ITransactionMonitorService>();
        }

        [Fact]
        public async Task TransactionFailedShouldManageAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 5;
            var failedReTryTimes = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true, includeRegistry: true);
            await dbContext.SaveChangesAsync();

            var registries = await dbContext.TransactionRegistries.Take(2).ToListAsync();
            var trackingToCancell = registries[0].TrackingId;
            var trackingToRetry = registries[1].TrackingId;
            registries[0].SetToRegistryError(TransactionErrorReason.GetTrasactionReceiptExpection);
            registries[0].SetWaitingToCancel();
            registries[1].SetToPool();
            registries[1].SetToRegistryError(TransactionErrorReason.UnableToSendTransactionOnChain);
            registries[1].SetWaitingToReTry();
            var pool = await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            pool.SetLocked(Guid.NewGuid());
            pool.UnlockFromError(TransactionErrorReason.InsertTransactionExpection);
            pool.SetLocked(Guid.NewGuid());
            pool.SetStatusError();
            await dbContext.SaveChangesAsync();

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionWaitingToReProcessAsync(max))
                .Returns(Task.FromResult((IEnumerable<TransactionRegistry>)registries));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionTriageAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionTriage>)await dbContext.TransactionTriages.Where(tt => tt.TrackingIdentify == trackingToCancell || tt.TrackingIdentify == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPoolAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToCancell || tt.TrackingId == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPendingAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPending>)await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToCancell || tt.TrackingId == trackingToRetry).ToListAsync()));

            var transactionFailedUseCase = new TransactionFailedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionFailedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            await transactionFailedUseCase.ManageAsync(max, failedReTryTimes);


            //Assert
            var trackingCancelled = await dbContext.TransactionRegistries.Where(tr => tr.TrackingId == trackingToCancell).FirstAsync();
            Assert.Equal(RegistryStatus.CanceledDueToError, trackingCancelled.Status);
            Assert.Equal(TransactionStep.Completed, trackingCancelled.TransactionStep);

            var trackingRetry = await dbContext.TransactionRegistries.Where(tr => tr.TrackingId == trackingToRetry).FirstAsync();
            var poolRetry = await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            Assert.Equal(RegistryStatus.InProgress, trackingRetry.Status);
            Assert.Equal(TransactionStep.Pool, trackingRetry.TransactionStep);
            Assert.Equal(0, poolRetry.ErrorTimes);
            Assert.False(poolRetry.Locked);
            Assert.Equal(PoolStatus.WaitingForWorker, poolRetry.Status);
        }

        [Theory]
        [InlineData(TransactionErrorReason.UnableToSendTransactionOnChain)]
        [InlineData(TransactionErrorReason.InsertTransactionExpection)]
        public async Task TransactionFailedInPoolShouldManageReTryExpectionAsync(TransactionErrorReason transactionErrorReason)
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 5;
            var failedReTryTimes = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true, includeRegistry: true);
            await dbContext.SaveChangesAsync();

            var registries = await dbContext.TransactionRegistries.Take(1).ToListAsync();
            var trackingToRetry = registries[0].TrackingId;
            registries[0].SetToPool();
            registries[0].SetToRegistryError(transactionErrorReason);
            registries[0].SetWaitingToReTry();
            var pool = await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            pool.SetLocked(Guid.NewGuid());
            pool.UnlockFromError(TransactionErrorReason.InsertTransactionExpection);
            pool.SetLocked(Guid.NewGuid());
            pool.SetStatusError();
            await dbContext.SaveChangesAsync();

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionWaitingToReProcessAsync(max))
                .Returns(Task.FromResult((IEnumerable<TransactionRegistry>)registries));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionTriageAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionTriage>)await dbContext.TransactionTriages.Where(tt => tt.TrackingIdentify == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPoolAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPendingAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPending>)await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).ToListAsync()));

            var transactionFailedUseCase = new TransactionFailedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionFailedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            await transactionFailedUseCase.ManageAsync(max, failedReTryTimes);


            //Assert
            var trackingRetry = await dbContext.TransactionRegistries.Where(tr => tr.TrackingId == trackingToRetry).FirstAsync();
            var poolRetry = await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            Assert.Equal(RegistryStatus.InProgress, trackingRetry.Status);
            Assert.Equal(TransactionStep.Pool, trackingRetry.TransactionStep);
            Assert.Equal(0, poolRetry.ErrorTimes);
            Assert.False(poolRetry.Locked);
            Assert.Equal(PoolStatus.WaitingForWorker, poolRetry.Status);
        }

        [Theory]
        [InlineData(TransactionErrorReason.TransactionNotFound)]
        [InlineData(TransactionErrorReason.TransactionFinalizedInError)]
        public async Task TransactionErrorShouldManageAgainInPoolAsync(TransactionErrorReason transactionErrorReason)
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 5;
            var failedReTryTimes = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true, includeRegistry: true);
            await dbContext.SaveChangesAsync();

            var registries = await dbContext.TransactionRegistries.Take(1).ToListAsync();
            var trackingToRetry = registries[0].TrackingId;
            registries[0].SetToPending("0xsssss", "http://endpoint.ext");
            registries[0].SetToRegistryError(transactionErrorReason);
            registries[0].SetWaitingToReTry();
            var pending = await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            pending.SetLocked(Guid.NewGuid());
            pending.UnlockFromError(TransactionErrorReason.UnableToWatchTransactionOnChain);
            pending.SetLocked(Guid.NewGuid());
            pending.SetStatusError();
            await dbContext.SaveChangesAsync();

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionWaitingToReProcessAsync(max))
                .Returns(Task.FromResult((IEnumerable<TransactionRegistry>)registries));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionTriageAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionTriage>)await dbContext.TransactionTriages.Where(tt => tt.TrackingIdentify == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPoolAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPendingAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPending>)await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).ToListAsync()));

            var transactionFailedUseCase = new TransactionFailedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionFailedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            await transactionFailedUseCase.ManageAsync(max, failedReTryTimes);


            //Assert
            var trackingRetry = await dbContext.TransactionRegistries.Where(tr => tr.TrackingId == trackingToRetry).FirstAsync();
            var poolRetry = await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            var pendingRetry = await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).FirstOrDefaultAsync();
            Assert.Equal(RegistryStatus.InProgress, trackingRetry.Status);
            Assert.Equal(TransactionStep.Pool, trackingRetry.TransactionStep);

            Assert.Equal(0, poolRetry.ErrorTimes);
            Assert.False(poolRetry.Locked);
            Assert.Equal(PoolStatus.WaitingForWorker, poolRetry.Status);

            Assert.Null(pendingRetry);
        }

        [Theory]
        [InlineData(TransactionErrorReason.GetTrasactionReceiptExpection)]
        [InlineData(TransactionErrorReason.UnableToWatchTransactionOnChain)]
        public async Task TransactionFailedInPendingShouldManageReTryAsync(TransactionErrorReason transactionErrorReason)
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 5;
            var failedReTryTimes = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true, includeRegistry: true);
            await dbContext.SaveChangesAsync();

            var registries = await dbContext.TransactionRegistries.Take(1).ToListAsync();
            var trackingToRetry = registries[0].TrackingId;
            registries[0].SetToPending("0xsssss", "http://endpoint.ext");
            registries[0].SetToRegistryError(transactionErrorReason);
            registries[0].SetWaitingToReTry();
            var pending = await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            pending.SetLocked(Guid.NewGuid());
            pending.UnlockFromError(TransactionErrorReason.UnableToWatchTransactionOnChain);
            pending.SetLocked(Guid.NewGuid());
            pending.SetStatusError();
            await dbContext.SaveChangesAsync();

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionWaitingToReProcessAsync(max))
                .Returns(Task.FromResult((IEnumerable<TransactionRegistry>)registries));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionTriageAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionTriage>)await dbContext.TransactionTriages.Where(tt => tt.TrackingIdentify == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPoolAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)await dbContext.TransactionPools.Where(tt => tt.TrackingId == trackingToRetry).ToListAsync()));

            mockTransactionMonitorService
                .Setup(m => m.GetTransactionPendingAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult((IEnumerable<TransactionPending>)await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).ToListAsync()));

            var transactionFailedUseCase = new TransactionFailedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionFailedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            await transactionFailedUseCase.ManageAsync(max, failedReTryTimes);


            //Assert
            var trackingRetry = await dbContext.TransactionRegistries.Where(tr => tr.TrackingId == trackingToRetry).FirstAsync();
            var pendingRetry = await dbContext.TransactionPendings.Where(tt => tt.TrackingId == trackingToRetry).FirstAsync();
            Assert.Equal(RegistryStatus.InProgress, trackingRetry.Status);
            Assert.Equal(TransactionStep.Pending, trackingRetry.TransactionStep);
            Assert.Equal(0, pendingRetry.ErrorTimes);
            Assert.False(pendingRetry.Locked);
            Assert.Equal(PendingStatus.WaitingForWorker, pendingRetry.Status);
        }

    }
}
