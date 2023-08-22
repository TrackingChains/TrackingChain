using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorCore.UseCases;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.TransactionMonitor
{
#pragma warning disable CA1001 // Not need in unit test
    public class TransactionDeleterUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;

        public TransactionDeleterUseCaseTest()
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
        }

        [Fact]
        public async Task TransactionDeleterShouldRemoveTriageCompletedAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var triages = await dbContext.TransactionTriages.Take(5).ToListAsync();
            foreach (var triage in triages)
                triage.SetCompleted();
            await dbContext.SaveChangesAsync();

            var transactionDeleterUseCase = new TransactionDeleterUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionDeleterUseCase>>());


            //Act
            var deletedResult = await transactionDeleterUseCase.RunAsync(max);


            //Assert
            Assert.Equal(3, deletedResult);
            var triageIds = await dbContext.TransactionTriages.Select(tp => tp.TrackingIdentify).ToListAsync();
            Assert.Equal(7, triageIds.Count);
            Assert.DoesNotContain(triageIds, id => id == triages[0].TrackingIdentify);
            Assert.DoesNotContain(triageIds, id => id == triages[1].TrackingIdentify);
            Assert.DoesNotContain(triageIds, id => id == triages[2].TrackingIdentify);
        }

        [Fact]
        public async Task TransactionDeleterShouldRemovePoolCompletedAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var pools = await dbContext.TransactionPools.Take(5).ToListAsync();
            foreach (var pool in pools)
                pool.SetCompleted();
            await dbContext.SaveChangesAsync();

            var transactionDeleterUseCase = new TransactionDeleterUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionDeleterUseCase>>());


            //Act
            var deletedResult = await transactionDeleterUseCase.RunAsync(max);


            //Assert
            Assert.Equal(3, deletedResult);
            var poolIds = await dbContext.TransactionPools.Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(7, poolIds.Count);
            Assert.DoesNotContain(poolIds, id => id == pools[0].TrackingId);
            Assert.DoesNotContain(poolIds, id => id == pools[1].TrackingId);
            Assert.DoesNotContain(poolIds, id => id == pools[2].TrackingId);
        }

        [Fact]
        public async Task TransactionDeleterShouldRemovePendingCompletedAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var pendings = await dbContext.TransactionPendings.Take(5).ToListAsync();
            foreach (var pending in pendings)
                pending.SetCompleted();
            await dbContext.SaveChangesAsync();

            var transactionDeleterUseCase = new TransactionDeleterUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionDeleterUseCase>>());


            //Act
            var deletedResult = await transactionDeleterUseCase.RunAsync(max);


            //Assert
            Assert.Equal(3, deletedResult);
            var pendingIds = await dbContext.TransactionPendings.Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(7, pendingIds.Count);
            Assert.DoesNotContain(pendingIds, id => id == pendings[0].TrackingId);
            Assert.DoesNotContain(pendingIds, id => id == pendings[1].TrackingId);
            Assert.DoesNotContain(pendingIds, id => id == pendings[2].TrackingId);
        }

        [Fact]
        public async Task TransactionDeleterShouldRemoveAllCompletedAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var max = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            var triages = await dbContext.TransactionTriages.Take(5).ToListAsync();
            foreach (var triage in triages)
                triage.SetCompleted();
            var pools = await dbContext.TransactionPools.Take(5).ToListAsync();
            foreach (var pool in pools)
                pool.SetCompleted();
            var pendings = await dbContext.TransactionPendings.Take(5).ToListAsync();
            foreach (var pending in pendings)
                pending.SetCompleted();
            await dbContext.SaveChangesAsync();

            var transactionDeleterUseCase = new TransactionDeleterUseCase(
                dbContext,
                Mock.Of<ILogger<TransactionDeleterUseCase>>());


            //Act
            var deletedResult = await transactionDeleterUseCase.RunAsync(max);


            //Assert

            //triage
            Assert.Equal(3, deletedResult);
            var triageIds = await dbContext.TransactionTriages.Select(tp => tp.TrackingIdentify).ToListAsync();
            Assert.Equal(7, triageIds.Count);
            Assert.DoesNotContain(triageIds, id => id == triages[0].TrackingIdentify);
            Assert.DoesNotContain(triageIds, id => id == triages[1].TrackingIdentify);
            Assert.DoesNotContain(triageIds, id => id == triages[2].TrackingIdentify);
            //pool
            Assert.Equal(3, deletedResult);
            var poolIds = await dbContext.TransactionPools.Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(7, poolIds.Count);
            Assert.DoesNotContain(poolIds, id => id == pools[0].TrackingId);
            Assert.DoesNotContain(poolIds, id => id == pools[1].TrackingId);
            Assert.DoesNotContain(poolIds, id => id == pools[2].TrackingId);
            //pending
            Assert.Equal(3, deletedResult);
            var pendingIds = await dbContext.TransactionPendings.Select(tp => tp.TrackingId).ToListAsync();
            Assert.Equal(7, pendingIds.Count);
            Assert.DoesNotContain(pendingIds, id => id == pools[0].TrackingId);
            Assert.DoesNotContain(pendingIds, id => id == pendings[1].TrackingId);
            Assert.DoesNotContain(pendingIds, id => id == pendings[2].TrackingId);
        }
    }
}
