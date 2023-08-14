using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionWatcherCore.Services;
using TrackingChain.TransactionWatcherCore.UseCases;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.TransactionWatcher
{

#pragma warning disable CA1001 // Not need in unit test
    public class PendingTransactionWatcherUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly IPendingTransactionWatcherUseCase pendingTransactionWatcherUseCase;
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<IAccountService> mockAccountService;
        private readonly Mock<IBlockchainService>[] blockchainServices;
        private readonly Mock<ITransactionWatcherService> mockTransactionWatcherService;

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

            //account service
            mockAccountService = new Mock<IAccountService>();

            //blockchain service
            blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
            var i = 0;
            foreach (var blockchainService in blockchainServices)
            {
                blockchainService
                    .Setup(m => m.InsertTrackingAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ContractExtraInfo>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(i % 2 == 0 ? Task.FromResult("0x1234567890") : Task.FromResult("0x0987654321"))
                    .Verifiable();

                if (i % 2 == 0)
                    blockchainService
                        .SetupGet(m => m.ProviderType)
                        .Returns(ChainType.EVM);
                else
                    blockchainService
                        .SetupGet(m => m.ProviderType)
                        .Returns(ChainType.Substrate);
                i++;
            }

            //transaction generator service
            mockTransactionWatcherService = new Mock<ITransactionWatcherService>();

            pendingTransactionWatcherUseCase = new PendingTransactionWatcherUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PendingTransactionWatcherUseCase>>(),
                mockTransactionWatcherService.Object);
        }

        [Fact]
        public async Task AddTransactionAsyncShouldGetGuidAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var maxConcurrentThread = 3;
            var reTryAfterSeconds = 6;
            var saveAsErrorAfterSeconds = 900;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(10, primaryProfile, secondaryProfile, dbContext, includePools: true);
            await dbContext.SaveChangesAsync();

            //mock
            var primaryAccount = await dbContext.Accounts.FirstAsync();
            mockAccountService
                .Setup(m => m.GetAccountAsync(primaryProfile))
                .Returns(Task.FromResult(primaryAccount));
            var primaryPendings = (await dbContext.TransactionPools
                .Take(maxConcurrentThread)
                .ToListAsync())
                .Select(tp => EntityCreator.ConvertToPending(tp, "txHash"));
            mockTransactionWatcherService
                .Setup(m => m.GetTransactionToCheckAsync(maxConcurrentThread, primaryProfile))
                .Returns(Task.FromResult(primaryPendings));
            mockTransactionWatcherService
                .Setup(m => m.SetToRegistryCompletedAsync(It.IsAny<Guid>(), It.IsAny<TransactionDetail>()))
                .Returns(Task.FromResult(EntityCreator.ConvertToRegistry(primaryPendings.First())));


            //Act
            var dequedResult = await pendingTransactionWatcherUseCase.CheckTransactionStatusAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, saveAsErrorAfterSeconds);


            //Assert
            Assert.True(dequedResult);
        }

    }
}
