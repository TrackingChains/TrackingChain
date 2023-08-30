using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorCore.Services;
using TrackingChain.TransactionMonitorCore.UseCases;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.TransactionMonitor
{
#pragma warning disable CA1001 // Not need in unit test
    public class TransactionLockedUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<ITransactionMonitorService> mockTransactionMonitorService;

        public TransactionLockedUseCaseTest()
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
        public async Task TransactionLockedShouldUnlockPendindsAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 3;
            var timeOutSeconds = 1;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var pendings = await dbContext.TransactionPendings.Take(5).ToListAsync();
            foreach (var pending in pendings)
                pending.SetLocked(Guid.NewGuid());
            await dbContext.SaveChangesAsync();

            Thread.Sleep(1100);

            mockTransactionMonitorService
                .Setup(m => m.GetPendingLockedInTimeoutAsync(max, timeOutSeconds))
                .Returns(Task.FromResult(pendings.Take(max)));

            var transactionLockedUseCase = new TransactionLockedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionLockedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            var processedResult = await transactionLockedUseCase.ReProcessAsync(max, timeOutSeconds, timeOutSeconds);


            //Assert
            Assert.Equal(3, processedResult);
            var pendingLockIds = await dbContext.TransactionPendings.Where(tp=>tp.Locked).Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(2, pendingLockIds.Count);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[0].TrackingId);
            Assert.Equal(1, pendings[0].ErrorTimes);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[1].TrackingId);
            Assert.Equal(1, pendings[1].ErrorTimes);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[2].TrackingId);
            Assert.Equal(1, pendings[2].ErrorTimes);
        }

        [Fact]
        public async Task TransactionLockedShouldUnlockPoolsAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 3;
            var timeOutSeconds = 1;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var pools = await dbContext.TransactionPools.Take(5).ToListAsync();
            foreach (var pool in pools)
                pool.SetLocked(Guid.NewGuid());
            await dbContext.SaveChangesAsync();

            Thread.Sleep(1100);

            mockTransactionMonitorService
                .Setup(m => m.GetPoolLockedInTimeoutAsync(max, timeOutSeconds))
                .Returns(Task.FromResult(pools.Take(max)));

            var transactionLockedUseCase = new TransactionLockedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionLockedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            var processedResult = await transactionLockedUseCase.ReProcessAsync(max, timeOutSeconds, timeOutSeconds);


            //Assert
            Assert.Equal(3, processedResult);
            var poolLockIds = await dbContext.TransactionPools.Where(tp => tp.Locked).Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(2, poolLockIds.Count);
            Assert.DoesNotContain(poolLockIds, id => id == pools[0].TrackingId);
            Assert.Equal(1, pools[0].ErrorTimes);
            Assert.DoesNotContain(poolLockIds, id => id == pools[1].TrackingId);
            Assert.Equal(1, pools[1].ErrorTimes);
            Assert.DoesNotContain(poolLockIds, id => id == pools[2].TrackingId);
            Assert.Equal(1, pools[2].ErrorTimes);
        }

        [Fact]
        public async Task TransactionLockedShouldUnlockPoolsPendingsAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 8;
            var timeOutSeconds = 1;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var pools = await dbContext.TransactionPools.Take(5).ToListAsync();
            foreach (var pool in pools)
                pool.SetLocked(Guid.NewGuid());
            await dbContext.SaveChangesAsync();

            var pendings = await dbContext.TransactionPendings.Take(5).ToListAsync();
            foreach (var pending in pendings)
                pending.SetLocked(Guid.NewGuid());
            await dbContext.SaveChangesAsync();

            Thread.Sleep(1100);

            mockTransactionMonitorService
                .Setup(m => m.GetPoolLockedInTimeoutAsync(max - 5, timeOutSeconds))
                .Returns(Task.FromResult(pools.Take(max - 5)));

            mockTransactionMonitorService
                .Setup(m => m.GetPendingLockedInTimeoutAsync(max, timeOutSeconds))
                .Returns(Task.FromResult(pendings.Take(max)));

            var transactionLockedUseCase = new TransactionLockedUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionLockedUseCase>>(),
                mockTransactionMonitorService.Object);


            //Act
            var processedResult = await transactionLockedUseCase.ReProcessAsync(max, timeOutSeconds, timeOutSeconds);


            //Assert
            Assert.Equal(8, processedResult);
            //pool
            var poolLockIds = await dbContext.TransactionPools.Where(tp => tp.Locked).Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(2, poolLockIds.Count);
            Assert.DoesNotContain(poolLockIds, id => id == pools[0].TrackingId);
            Assert.Equal(1, pools[0].ErrorTimes);
            Assert.DoesNotContain(poolLockIds, id => id == pools[1].TrackingId);
            Assert.Equal(1, pools[1].ErrorTimes);
            Assert.DoesNotContain(poolLockIds, id => id == pools[2].TrackingId);
            Assert.Equal(1, pools[2].ErrorTimes);
            //pendings
            var pendingLockIds = await dbContext.TransactionPendings.Where(tp => tp.Locked).Select(tp => tp.TrackingId).ToListAsync();
            Assert.Empty(pendingLockIds);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[0].TrackingId);
            Assert.Equal(1, pendings[0].ErrorTimes);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[1].TrackingId);
            Assert.Equal(1, pendings[1].ErrorTimes);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[2].TrackingId);
            Assert.Equal(1, pendings[2].ErrorTimes);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[3].TrackingId);
            Assert.Equal(1, pendings[3].ErrorTimes);
            Assert.DoesNotContain(pendingLockIds, id => id == pendings[4].TrackingId);
            Assert.Equal(1, pendings[4].ErrorTimes);
        }
    }
}
