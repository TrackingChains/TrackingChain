using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TransactionWatcherCore.Services;
using TrackingChain.TransactionWatcherCore.UseCases;
using Xunit;

namespace TrackingChain.UnitTest.TransactionWatcher
{/*
#pragma warning disable CA1001 // Not need in unit test
    public class PendingTransactionWatcherUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly IPendingTransactionWatcherUseCase pendingTransactionWatcherUseCase;
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<AccountService> mockAccountService;
        private readonly Mock<EthereumService> ethereumService;
        private readonly Mock<TransactionWatcherService> mockTransactionWatcherService;

        public PendingTransactionWatcherUseCaseTest()
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

            //account service
            mockAccountService = new Mock<AccountService>();

            pendingTransactionWatcherUseCase = new PendingTransactionWatcherUseCase(
                mockAccountService.Object,
            mockTransactionTriageService.Object,
            Mock.Of<ILogger<PendingTransactionWatcherUseCase>>(),
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

    }*/
}


