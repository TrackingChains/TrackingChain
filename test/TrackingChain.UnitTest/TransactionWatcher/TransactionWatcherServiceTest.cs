using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionWatcherCore.Services;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.TransactionWatcher
{
#pragma warning disable CA1001 // Not need in unit test
    public class TransactionWatcherServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ITransactionWatcherService transactionWatcherService;
        private readonly ApplicationDbContext dbContext;

        public TransactionWatcherServiceTest()
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

            transactionWatcherService = new TransactionWatcherService(
                dbContext,
                Mock.Of<ILogger<TransactionWatcherService>>());
        }

        [Fact]
        public async Task ShouldBeSetTransactionPoolCompletedAsync()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(5);
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");

            var transactionPools = EntityCreator.CreateTransactionPool(transactionTriages);
            dbContext.TransactionPools.AddRange(transactionPools);
            dbContext.SaveChanges();

            //Act
            var txPool = await transactionWatcherService.SetTransactionPoolCompletedAsync(twoTriage.TrackingIdentify);
            await dbContext.SaveChangesAsync();


            //Assert
            Assert.True(txPool.Completed);
        }

        [Fact]
        public async Task ShouldBeSetTransactionTriageCompletedAsync()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(5);
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");
            dbContext.TransactionTriages.AddRange(transactionTriages);
            dbContext.SaveChanges();

            //Act
            var txTriage = await transactionWatcherService.SetTransactionTriageCompletedAsync(twoTriage.TrackingIdentify);
            await dbContext.SaveChangesAsync();


            //Assert
            Assert.True(txTriage.Completed);
        }

        [Fact]
        public async Task ShouldGetOnlyUnlockedTransactionForProfileAsync()
        {
            //Arrange
            var currentProfileAccount = Guid.NewGuid();
            var secondaryProfileAccount = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(
                10,
                currentProfileAccount,
                secondaryProfileAccount,
                dbContext,
                includePools: true);

            //pool
            var txPendings = EntityCreator.CreateTransactionPending(dbContext.TransactionTriages, forceWatchingFrom: DateTime.UtcNow.AddSeconds(-10));
            txPendings.First(tp => tp.Code == "Code6").SetLoked(secondaryProfileAccount);
            txPendings.First(tp => tp.Code == "Code8").SetLoked(secondaryProfileAccount);

            dbContext.TransactionPendings.AddRange(txPendings);
            await dbContext.SaveChangesAsync();


            //Act
            var txPendingsCaseOne = await transactionWatcherService.GetTransactionToCheckAsync(2, currentProfileAccount);
            var txPendingsCaseTwo = await transactionWatcherService.GetTransactionToCheckAsync(5, currentProfileAccount);
            var txPendingsCaseThree = await transactionWatcherService.GetTransactionToCheckAsync(7, currentProfileAccount);
            var txPendingsCaseFour = await transactionWatcherService.GetTransactionToCheckAsync(7, secondaryProfileAccount);
            var txPendingsCaseFive = await transactionWatcherService.GetTransactionToCheckAsync(7, Guid.NewGuid());


            //Assert
            //case one
            Assert.Equal(2, txPendingsCaseOne.Count());

            //case two
            Assert.Equal(5, txPendingsCaseTwo.Count());
            Assert.Contains(txPendingsCaseTwo, tp => tp.Code == "Code1");
            Assert.Contains(txPendingsCaseTwo, tp => tp.Code == "Code3");
            Assert.Contains(txPendingsCaseTwo, tp => tp.Code == "Code5");
            Assert.Contains(txPendingsCaseTwo, tp => tp.Code == "Code7");
            Assert.Contains(txPendingsCaseTwo, tp => tp.Code == "Code9");

            //case three
            Assert.Equal(5, txPendingsCaseThree.Count());
            Assert.Contains(txPendingsCaseThree, tp => tp.Code == "Code1");
            Assert.Contains(txPendingsCaseThree, tp => tp.Code == "Code3");
            Assert.Contains(txPendingsCaseThree, tp => tp.Code == "Code5");
            Assert.Contains(txPendingsCaseThree, tp => tp.Code == "Code7");
            Assert.Contains(txPendingsCaseThree, tp => tp.Code == "Code9");

            //case four
            Assert.Equal(3, txPendingsCaseFour.Count());
            Assert.Contains(txPendingsCaseFour, tp => tp.Code == "Code2");
            Assert.Contains(txPendingsCaseFour, tp => tp.Code == "Code4");
            Assert.Contains(txPendingsCaseFour, tp => tp.Code == "Code10");

            //case five
            Assert.Empty(txPendingsCaseFive);
        }

        [Fact]
        public async Task GetOnlyUnlockedTransactionShouldFilterWatchingFromAsync()
        {
            //Arrange
            var currentProfileAccount = Guid.NewGuid();
            var secondaryProfileAccount = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(
                10,
                currentProfileAccount,
                secondaryProfileAccount,
                dbContext,
                includePools: true);

            //pool
            var txPendings = EntityCreator.CreateTransactionPending(dbContext.TransactionTriages, forceWatchingFrom: DateTime.UtcNow.AddSeconds(100));
            txPendings.First(tp => tp.Code == "Code6").SetLoked(secondaryProfileAccount);
            txPendings.First(tp => tp.Code == "Code8").SetLoked(secondaryProfileAccount);

            dbContext.TransactionPendings.AddRange(txPendings);
            await dbContext.SaveChangesAsync();


            //Act
            var txPendingsCaseOne = await transactionWatcherService.GetTransactionToCheckAsync(2, currentProfileAccount);
            var txPendingsCaseTwo = await transactionWatcherService.GetTransactionToCheckAsync(5, currentProfileAccount);
            var txPendingsCaseThree = await transactionWatcherService.GetTransactionToCheckAsync(7, currentProfileAccount);
            var txPendingsCaseFour = await transactionWatcherService.GetTransactionToCheckAsync(7, secondaryProfileAccount);
            var txPendingsCaseFive = await transactionWatcherService.GetTransactionToCheckAsync(7, Guid.NewGuid());


            //Assert
            //case one
            Assert.Empty(txPendingsCaseOne);

            //case two
            Assert.Empty(txPendingsCaseTwo);

            //case three
            Assert.Empty(txPendingsCaseThree);

            //case four
            Assert.Empty(txPendingsCaseFour);

            //case five
            Assert.Empty(txPendingsCaseFive);
        }

        [Fact]
        public async Task SetToPendinglShouldSetRegistryInComplitedAsync()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(5);
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");

            var transactionRegistries = EntityCreator.CreateTransactionRegistry(transactionTriages);
            dbContext.TransactionRegistries.AddRange(transactionRegistries);
            dbContext.SaveChanges();
            var transactionDetail = new TransactionDetail("1", "12", "", "987", "12", "", "0x9876543", "648", true, "", "0x123456789");

            //Act
            var txRegistry = await transactionWatcherService.SetToRegistryAsync(twoTriage.TrackingIdentify, transactionDetail);
            await dbContext.SaveChangesAsync();


            //Assert
            Assert.Equal(TransactionStep.Completed, txRegistry.TransactionStep);
            Assert.Equal(transactionDetail.BlockHash, txRegistry.ReceiptBlockHash);
            Assert.Equal(transactionDetail.BlockNumber, txRegistry.ReceiptBlockNumber);
            Assert.Equal(transactionDetail.CumulativeGasUsed, txRegistry.ReceiptCumulativeGasUsed);
            Assert.Equal(transactionDetail.EffectiveGasPrice, txRegistry.ReceiptEffectiveGasPrice);
            Assert.Equal(transactionDetail.From, txRegistry.ReceiptFrom);
            Assert.Equal(transactionDetail.GasUsed, txRegistry.ReceiptGasUsed);
            Assert.Equal(transactionDetail.Successful, txRegistry.ReceiptSuccessful);
            Assert.Equal(transactionDetail.To, txRegistry.ReceiptTo);
        }
    }
}
