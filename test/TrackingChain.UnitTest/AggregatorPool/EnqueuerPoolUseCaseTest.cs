using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.AggregatorPoolCore.Services;
using TrackingChain.AggregatorPoolCore.UseCases;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.AggregatorPool
{
#pragma warning disable CA1001 // Not need in unit test
    public class EnqueuerPoolUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly EnqueuerPoolUseCase enqueuerPoolUseCase;
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<IAggregatorService> mockAggregatorService;

        public EnqueuerPoolUseCaseTest()
        {
            //db context
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

            //unit of work
            IUnitOfWork unitOfWork = new UnitOfWork(Mock.Of<ILogger<UnitOfWork>>(), dbContext);

            //aggregator service
            mockAggregatorService = new Mock<IAggregatorService>();     

            enqueuerPoolUseCase = new EnqueuerPoolUseCase(
                mockAggregatorService.Object,
                Mock.Of<ILogger<EnqueuerPoolUseCase>>(),
                unitOfWork);
        }

        [Fact]
        public async Task EnqueueTransactionAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(5, primaryProfile, secondaryProfile, dbContext, includePools: true);
            await dbContext.SaveChangesAsync();

            IEnumerable<TransactionTriage> mockTriageResponse = await dbContext.TransactionTriages.Take(2).ToListAsync();
            mockAggregatorService.Setup(m => m.GetTransactionToEnqueueAsync(2, It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.FromResult(mockTriageResponse));

            IEnumerable<TransactionPool> mockPoolResponse = await dbContext.TransactionPools.Take(2).ToListAsync();
            mockAggregatorService.Setup(m => m.Enqueue(mockTriageResponse))
                .Returns(mockPoolResponse);

            //Act
            var enqueuedTriage = await enqueuerPoolUseCase.EnqueueTransactionAsync(2);


            //Assert
            Assert.Equal(2, enqueuedTriage);
        }

    }
}

