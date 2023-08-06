using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Triage
{
#pragma warning disable CA1001 // Not need in unit test
    public class AnalyticUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly IAnalyticUseCase analyticUseCase;
        private readonly ApplicationDbContext dbContext;

        public AnalyticUseCaseTest()
        {
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

            analyticUseCase = new AnalyticUseCase(
                Mock.Of<ILogger<AnalyticUseCase>>(),
                dbContext);
        }

        [Fact]
        public async Task GetTrackingSuccessfullyAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            foreach (var item in entityTransactionRegistries)
                item.SetToRegistry("1", "2", "3", "4", "5", "6", true, "7");
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var transactionRegistries = await analyticUseCase.GetTrackingSuccessfullyAsync(5, 1);


            //Assert
            Assert.Equal(5, transactionRegistries.Count());
        }

        [Fact]
        public async Task GetTrackingFailedsAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            foreach (var item in entityTransactionRegistries)
                item.SetToRegistry("1", "2", "3", "4", "5", "6", false, "7");
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var transactionRegistries = await analyticUseCase.GetTrackingFailedsAsync(5, 1);


            //Assert
            Assert.Equal(5, transactionRegistries.Count());
        }

        [Fact]
        public async Task GetTrackingTriagesAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var transactionRegistries = await analyticUseCase.GetTrackingTriagesAsync(5, 1);


            //Assert
            Assert.Equal(5, transactionRegistries.Count());
        }

        [Fact]
        public async Task GetTrackingPoolsAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            foreach (var item in entityTransactionRegistries)
                item.SetToPool();
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var transactionRegistries = await analyticUseCase.GetTrackingPoolsAsync(5, 1);


            //Assert
            Assert.Equal(5, transactionRegistries.Count());
        }

        [Fact]
        public async Task GetTrackingPendingsAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            foreach (var item in entityTransactionRegistries)
                item.SetToPending("0x2345678");
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var transactionRegistries = await analyticUseCase.GetTrackingPendingsAsync(5, 1);


            //Assert
            Assert.Equal(5, transactionRegistries.Count());
        }

        [Fact]
        public async Task GetTrackingShouldBeFindElementAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();
            var item = await dbContext.TransactionRegistries.FirstAsync();


            //Act
            var transactionRegistry = await analyticUseCase.GetTrackingAsync(item.TrackingId);


            //Assert
            Assert.Equal(item.TrackingId, transactionRegistry!.TrackingId);
        }

        [Fact]
        public async Task GetTrackingShouldBeReturNullAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var transactionRegistry = await analyticUseCase.GetTrackingAsync(Guid.NewGuid());


            //Assert
            Assert.Null(transactionRegistry);
        }

        [Fact]
        public async Task GetTrackingStatusShouldBeFindElementAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();
            var item = await dbContext.TransactionRegistries.FirstAsync();


            //Act
            var trackingStatus = await analyticUseCase.GetTrackingStatusAsync(item.TrackingId);


            //Assert
            Assert.Equal(item.TrackingId, trackingStatus!.TrackingId);
        }

        [Fact]
        public async Task GetTrackingStatusShouldBeReturNullAsync()
        {
            //Arrange
            var entityTransactionRegistries = EntityCreator.CreateTransactionRegistry(10);
            dbContext.TransactionRegistries.AddRange(entityTransactionRegistries);
            dbContext.SaveChanges();


            //Act
            var trackingStatus = await analyticUseCase.GetTrackingStatusAsync(Guid.NewGuid());


            //Assert
            Assert.Null(trackingStatus); ;
        }
    }
}