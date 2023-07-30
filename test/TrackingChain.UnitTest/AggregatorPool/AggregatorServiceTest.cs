using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.AggregatorPoolCore.Services;
using TrackingChain.Core.Helpers;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using Xunit;

namespace TrackingChain.Core.AggregatorPool
{
#pragma warning disable CA1001 // Not need in unit test
    public class AggregatorServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly AggregatorService aggregatorService;
        private readonly ApplicationDbContext dbContext;

        public AggregatorServiceTest()
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

            aggregatorService = new AggregatorService(
                dbContext,
                Mock.Of<ILogger<AggregatorService>>());
        }

        [Fact]
        public void EnqueueShouldCreatePool()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(2);
            var oneTriage = transactionTriages.First(i => i.Code == "Code1");
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");

            //Act
            var enqueuedTriage = aggregatorService.Enqueue(transactionTriages);


            //Assert
            Assert.Equal(2, enqueuedTriage.Count());

            var oneEnqueue = enqueuedTriage.First(i => i.Code == oneTriage.Code);
            Assert.Equal(oneTriage.DataValue, oneEnqueue.DataValue);
            Assert.Equal(oneTriage.ProfileGroupId, oneEnqueue.ProfileGroupId);
            Assert.False(oneEnqueue.Completed);
            Assert.Equal(oneTriage.ChainNumberId, oneEnqueue.ChainNumberId);
            Assert.Equal(oneTriage.ChainType, oneEnqueue.ChainType);
            Assert.False(oneEnqueue.Locked);
            Assert.Null(oneEnqueue.LockedBy);
            Assert.Null(oneEnqueue.LockedDated);
            Assert.Equal(0, oneEnqueue.Priority);
            Assert.Equal(oneTriage.SmartContractAddress, oneEnqueue.SmartContractAddress);
            Assert.Equal(oneTriage.SmartContractExtraInfo, oneEnqueue.SmartContractExtraInfo);
            Assert.NotEqual(Guid.Empty, oneEnqueue.TrackingId);

            var twoEnqueue = enqueuedTriage.First(i => i.Code == twoTriage.Code);
            Assert.Equal(twoTriage.DataValue, twoEnqueue.DataValue);
            Assert.Equal(twoTriage.ProfileGroupId, twoEnqueue.ProfileGroupId);
            Assert.False(twoEnqueue.Completed);
            Assert.Equal(twoTriage.ChainNumberId, twoEnqueue.ChainNumberId);
            Assert.Equal(twoTriage.ChainType, twoEnqueue.ChainType);
            Assert.False(twoEnqueue.Locked);
            Assert.Null(twoEnqueue.LockedBy);
            Assert.Null(twoEnqueue.LockedDated);
            Assert.Equal(0, twoEnqueue.Priority);
            Assert.Equal(twoTriage.SmartContractAddress, twoEnqueue.SmartContractAddress);
            Assert.Equal(twoTriage.SmartContractExtraInfo, twoEnqueue.SmartContractExtraInfo);
            Assert.NotEqual(Guid.Empty, twoEnqueue.TrackingId);
        }

        [Fact]
        public void EnqueueShouldSetTriageInPool()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(2);
            var oneTriage = transactionTriages.First(i => i.Code == "Code1");
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");


            //Act
            var enqueuedTriage = aggregatorService.Enqueue(transactionTriages);


            //Assert
            Assert.True(oneTriage.IsInPool);
            Assert.True(twoTriage.IsInPool);
        }

        [Fact]
        public async Task SetToPoolShouldSetRegistryInPoolAsync()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(5);
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");

            var transactionRegistries = EntityCreator.CreateTransactionRegistry(transactionTriages);
            dbContext.TransactionRegistries.AddRange(transactionRegistries);
            dbContext.SaveChanges();


            //Act
            var registries = await aggregatorService.SetToPoolAsync(new List<Guid> { twoTriage.TrackingIdentify });
            await dbContext.SaveChangesAsync();


            //Assert
            Assert.Single(registries);
            Assert.Equal(TransactionStep.Pool, registries.First().TransactionStep);
        }
    }
}
