using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TransactionWaitingCore.Services;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Triage
{
#pragma warning disable CA1001 // Not need in unit test
    public class TrackingEntryUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly TrackingEntryUseCase trackingEntryUseCase;
        private readonly ApplicationDbContext dbContext;

        public TrackingEntryUseCaseTest()
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

            IUnitOfWork unitOfWork = new UnitOfWork(Mock.Of<ILogger<UnitOfWork>>(), dbContext);

            //transaction triage service
            var mockTransactionTriageService = new Mock<ITransactionTriageService>();
            mockTransactionTriageService
                .Setup(m => m.AddTransactionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(EntityCreator.CreateTransactionTriage(1).First()));

            trackingEntryUseCase = new TrackingEntryUseCase(
                Mock.Of<ILogger<TrackingEntryUseCase>>(),
                mockTransactionTriageService.Object,
                unitOfWork);
        }

        [Fact]
        public async Task AddTransactionAsyncShouldGetGuidAsync()
        {
            //Arrange
            var auth = "AuthForTest";
            var code = "CodeForTest";
            var dataValue = "DatValueForTest";
            var category = "CategoryForTest";


            //Act
            var guidTracking = await trackingEntryUseCase.AddTransactionAsync(auth, code, dataValue, category);


            //Assert
            Assert.NotEqual(Guid.Empty, guidTracking);
        }

    }
}

