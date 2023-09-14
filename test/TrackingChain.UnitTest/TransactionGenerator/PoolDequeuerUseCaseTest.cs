using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
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
using TrackingChain.TransactionGeneratorCore.Services;
using TrackingChain.TransactionGeneratorCore.UseCases;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.TransactionGenerator
{
#pragma warning disable CA1001 // Not need in unit test
    public class PoolDequeuerUseCaseTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<IAccountService> mockAccountService;
        private readonly Mock<ITransactionGeneratorService> mockTransactionGeneratorService;

        public PoolDequeuerUseCaseTest()
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
            mockTransactionGeneratorService = new Mock<ITransactionGeneratorService>();
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
            var primaryPools = await dbContext.TransactionPools.Take(maxConcurrentThread).ToListAsync();
            mockTransactionGeneratorService
                .Setup(m => m.GetAvaiableTransactionPoolAsync(maxConcurrentThread, primaryProfile))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)primaryPools));
            mockTransactionGeneratorService
                .Setup(m => m.AddTransactionPendingFromPool(It.IsAny<TransactionPool>(), It.IsAny<string>()))
                .Returns(EntityCreator.ConvertToPending(primaryPools.First(), "hashTx"));

            //blockchain service
            var blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
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
                    .ReturnsAsync((i % 2 == 0) ? 
                        (TransactionDetail?)new TransactionDetail("0x1234567890") : 
                        (TransactionDetail?)new TransactionDetail("0x0987654321"))
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
            var poolDequeuerUseCase = new PoolDequeuerUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PoolDequeuerUseCase>>(),
                mockTransactionGeneratorService.Object);


            //Act
            var dequedResult = await poolDequeuerUseCase.DequeueTransactionAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, saveAsErrorAfterSeconds);


            //Assert
            Assert.NotEqual(Guid.Empty, dequedResult);
        }

        [Fact]
        public async Task DequeueTransactionFailedShouldSetInWaitingForWorkerAsync()
        {
            //Arrange
            var blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
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
                    .Throws(new InvalidOperationException());

                blockchainService
                        .SetupGet(m => m.ProviderType)
                        .Returns(ChainType.EVM);
            }

            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var maxConcurrentThread = 3;
            var reTryAfterSeconds = 6;
            var errorAfterReTry = 3;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(1, primaryProfile, secondaryProfile, dbContext, includePools: true);
            await dbContext.SaveChangesAsync();

            //mock
            var primaryAccount = await dbContext.Accounts.FirstAsync();
            mockAccountService
                .Setup(m => m.GetAccountAsync(primaryProfile))
                .Returns(Task.FromResult(primaryAccount));
            var primaryPools = await dbContext.TransactionPools.Take(maxConcurrentThread).ToListAsync();
            mockTransactionGeneratorService
                .Setup(m => m.GetAvaiableTransactionPoolAsync(maxConcurrentThread, primaryProfile))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)primaryPools));
            mockTransactionGeneratorService
                .Setup(m => m.AddTransactionPendingFromPool(It.IsAny<TransactionPool>(), It.IsAny<string>()))
                .Returns(EntityCreator.ConvertToPending(primaryPools.First(), "hashTx"));

            var poolDequeuerUseCase = new PoolDequeuerUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PoolDequeuerUseCase>>(),
                mockTransactionGeneratorService.Object);


            //Act
            var dequedResult = await poolDequeuerUseCase.DequeueTransactionAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, errorAfterReTry);


            //Assert
            Assert.Equal(primaryPools.First().TrackingId, dequedResult);
            var trackPool = await dbContext.TransactionPools.FirstAsync(tr => tr.TrackingId == dequedResult);
            Assert.Equal(1, trackPool.ErrorTimes);
            Assert.Equal(PoolStatus.WaitingForWorker, trackPool.Status);
        }

        [Fact]
        public async Task DequeueTransactionFailedShouldSetInErrorWhenReachMaxLimitAsync()
        {
            //Arrange
            var blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
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
                    .Throws(new InvalidOperationException());

                blockchainService
                        .SetupGet(m => m.ProviderType)
                        .Returns(ChainType.EVM);
            }

            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            var maxConcurrentThread = 3;
            var reTryAfterSeconds = 6;
            var errorAfterReTry = 1;

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(1, primaryProfile, secondaryProfile, dbContext, includePools: true);
            await dbContext.SaveChangesAsync();

            //mock
            var primaryAccount = await dbContext.Accounts.FirstAsync();
            mockAccountService
                .Setup(m => m.GetAccountAsync(primaryProfile))
                .Returns(Task.FromResult(primaryAccount));
            var primaryPools = await dbContext.TransactionPools.Take(maxConcurrentThread).ToListAsync();
            mockTransactionGeneratorService
                .Setup(m => m.GetAvaiableTransactionPoolAsync(maxConcurrentThread, primaryProfile))
                .Returns(Task.FromResult((IEnumerable<TransactionPool>)primaryPools));
            mockTransactionGeneratorService
                .Setup(m => m.AddTransactionPendingFromPool(It.IsAny<TransactionPool>(), It.IsAny<string>()))
                .Returns(EntityCreator.ConvertToPending(primaryPools.First(), "hashTx"));

            var poolDequeuerUseCase = new PoolDequeuerUseCase(
                mockAccountService.Object,
                dbContext,
                blockchainServices.Select(mm => mm.Object),
                Mock.Of<ILogger<PoolDequeuerUseCase>>(),
                mockTransactionGeneratorService.Object);
            blockchainServices = new[] { new Mock<IBlockchainService>(), new Mock<IBlockchainService>(), };
            await poolDequeuerUseCase.DequeueTransactionAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, errorAfterReTry);


            //Act
            var dequedResult = await poolDequeuerUseCase.DequeueTransactionAsync(maxConcurrentThread, primaryProfile, reTryAfterSeconds, errorAfterReTry);


            //Assert
            Assert.Equal(primaryPools.First().TrackingId, dequedResult);
            var trackPool = await dbContext.TransactionPools.FirstAsync(tr => tr.TrackingId == dequedResult);
            Assert.Equal(2, trackPool.ErrorTimes);
            Assert.Equal(PoolStatus.Error, trackPool.Status);
        }
    }
}


