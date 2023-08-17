using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
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
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<IAccountService> mockAccountService;
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

            //transaction generator service
            mockTransactionWatcherService = new Mock<ITransactionWatcherService>();
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

            //blockchain service
            var blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
            var i = 0;
            foreach (var blockchainService in blockchainServices)
            {
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

            var pendingTransactionWatcherUseCase = new PendingTransactionWatcherUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PendingTransactionWatcherUseCase>>(),
                mockTransactionWatcherService.Object);


            //Act
            var dequedResult = await pendingTransactionWatcherUseCase.CheckTransactionStatusAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, saveAsErrorAfterSeconds);


            //Assert
            Assert.Equal(primaryPendings.First().TrackingId, dequedResult);
        }

        [Fact]
        public async Task CheckTransactionStatusFailedShouldSetInWaitingForWorkerAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var maxConcurrentThread = 3;
            var reTryAfterSeconds = 6;
            var errorAfterReTry = 2;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(1, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            //mock
            var primaryAccount = await dbContext.Accounts.FirstAsync();
            mockAccountService
                .Setup(m => m.GetAccountAsync(primaryProfile))
                .Returns(Task.FromResult(primaryAccount));
            var primaryPending = await dbContext.TransactionPendings.FirstAsync();
            IEnumerable<TransactionPending> transactionPendings = new List<TransactionPending> { primaryPending };
            mockTransactionWatcherService
                .Setup(m => m.GetTransactionToCheckAsync(maxConcurrentThread, primaryProfile))
                .Returns(Task.FromResult(transactionPendings));
            mockTransactionWatcherService
                .Setup(m => m.SetToRegistryCompletedAsync(It.IsAny<Guid>(), It.IsAny<TransactionDetail>()))
                .Returns(Task.FromResult(EntityCreator.ConvertToRegistry(primaryPending)));

            //blockchain service
            var blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
            foreach (var blockchainService in blockchainServices)
            {
                blockchainService
                    .Setup(m => m.GetTrasactionReceiptAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                    .Throws(new InvalidOperationException());

                blockchainService
                        .SetupGet(m => m.ProviderType)
                        .Returns(ChainType.EVM);
            }

            var pendingTransactionWatcherUseCase = new PendingTransactionWatcherUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PendingTransactionWatcherUseCase>>(),
                mockTransactionWatcherService.Object);


            //Act
            var dequedResult = await pendingTransactionWatcherUseCase.CheckTransactionStatusAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, errorAfterReTry);


            //Assert
            Assert.Equal(primaryPending.TrackingId, dequedResult);
            var trackPending = await dbContext.TransactionPendings.FirstAsync(tr => tr.TrackingId == dequedResult);
            Assert.Equal(1, trackPending.ErrorTimes);
            Assert.Equal(PendingStatus.WaitingForWorker, trackPending.Status);
        }

        [Fact]
        public async Task DequeueTransactionFailedFailedShouldSetInErrorWhenReachMaxLimitAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var maxConcurrentThread = 3;
            var reTryAfterSeconds = 6;
            var errorAfterReTry = 1;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(1, primaryProfile, secondaryProfile, dbContext, includePools: true, includePendings: true);
            await dbContext.SaveChangesAsync();

            //mock
            var primaryAccount = await dbContext.Accounts.FirstAsync();
            mockAccountService
                .Setup(m => m.GetAccountAsync(primaryProfile))
                .Returns(Task.FromResult(primaryAccount));
            var primaryPending = await dbContext.TransactionPendings.FirstAsync();
            IEnumerable<TransactionPending> transactionPendings = new List<TransactionPending> { primaryPending };
            mockTransactionWatcherService
                .Setup(m => m.GetTransactionToCheckAsync(maxConcurrentThread, primaryProfile))
                .Returns(Task.FromResult(transactionPendings));
            mockTransactionWatcherService
                .Setup(m => m.SetToRegistryCompletedAsync(It.IsAny<Guid>(), It.IsAny<TransactionDetail>()))
                .Returns(Task.FromResult(EntityCreator.ConvertToRegistry(primaryPending)));

            //blockchain service
            var blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
            foreach (var blockchainService in blockchainServices)
            {
                blockchainService
                    .Setup(m => m.GetTrasactionReceiptAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                    .Throws(new InvalidOperationException());

                blockchainService
                        .SetupGet(m => m.ProviderType)
                        .Returns(ChainType.EVM);
            }

            var pendingTransactionWatcherUseCase = new PendingTransactionWatcherUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PendingTransactionWatcherUseCase>>(),
                mockTransactionWatcherService.Object);
            await pendingTransactionWatcherUseCase.CheckTransactionStatusAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, errorAfterReTry);


            //Act
            var dequedResult = await pendingTransactionWatcherUseCase.CheckTransactionStatusAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, errorAfterReTry);


            //Assert
            Assert.Equal(primaryPending.TrackingId, dequedResult);
            var trackPending = await dbContext.TransactionPendings.FirstAsync(tr => tr.TrackingId == dequedResult);
            Assert.Equal(1, trackPending.ErrorTimes);
            Assert.Equal(PendingStatus.Error, trackPending.Status);
        }

    }
}
