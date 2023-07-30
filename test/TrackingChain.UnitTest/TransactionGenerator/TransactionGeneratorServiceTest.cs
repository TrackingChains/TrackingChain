using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Helpers;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionGeneratorCore.Services;
using Xunit;

namespace TrackingChain.Core.TransactionGenerator
{
#pragma warning disable CA1001 // Not need in unit test
    public class TransactionGeneratorServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly TransactionGeneratorService transactionGeneratorService;
        private readonly ApplicationDbContext dbContext;

        public TransactionGeneratorServiceTest()
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

            transactionGeneratorService = new TransactionGeneratorService(
                dbContext,
                Mock.Of<ILogger<TransactionGeneratorService>>());
        }

        [Fact]
        public async Task ShouldBeCreateTransactionPendingAsync()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(1);
            var txPool = EntityCreator.CreateTransactionPool(transactionTriages).First();
            var txHash = "12345-67890";

            //Act
            var txPending = transactionGeneratorService.AddTransactionPendingFromPool(txPool, txHash);
            await dbContext.SaveChangesAsync();


            //Assert
            Assert.Equal(txPool.TriageDate, txPending.TriageDate);
            Assert.Equal(txPool.DataValue, txPending.DataValue);
            Assert.Equal(txPool.ChainNumberId, txPending.ChainNumberId);
            Assert.Equal(txPool.ChainType, txPending.ChainType);
            Assert.Equal(txPool.Code, txPending.Code);
            Assert.Equal(txPool.ProfileGroupId, txPending.ProfileGroupId);
            Assert.Equal(txPool.SmartContractAddress, txPending.SmartContractAddress);
            Assert.Equal(txPool.SmartContractExtraInfo, txPending.SmartContractExtraInfo);
            Assert.Equal(txPool.SmartContractId, txPending.SmartContractId);
            Assert.Equal(txPool.TrackingId, txPending.TrackingId);
            Assert.Equal(txHash, txPending.TxHash);

            Assert.True(dbContext.TransactionPendings.Any(tp => tp.TxHash == txHash));
        }

        [Fact]
        public async Task ShouldGetOnlyUnlockedTransactionForProfileAsync()
        {
            //Arrange
            var currentProfileAccount = Guid.NewGuid();
            var secondaryProfileAccount = Guid.NewGuid();

            //smart contracts
            var smartContracts = EntityCreator.CreateSmartContract(2);
            dbContext.SmartContracts.AddRange(smartContracts);

            //profile group
            var profileGroupOne = new ProfileGroup(null, null, null, "test unit", smartContracts.ElementAt(0), 0);
            dbContext.ProfileGroups.Add(profileGroupOne);
            var profileGroupTwo = new ProfileGroup(null, null, null, "test unit", smartContracts.ElementAt(1), 0);
            dbContext.ProfileGroups.Add(profileGroupTwo);
            await dbContext.SaveChangesAsync();

            //account profile group
            var accountProfileGroupOne = new AccountProfileGroup(currentProfileAccount, profileGroupOne.Id, 0);
            dbContext.AccountProfileGroup.Add(accountProfileGroupOne);
            var accountProfileGroupTwo = new AccountProfileGroup(secondaryProfileAccount, profileGroupTwo.Id, 0);
            dbContext.AccountProfileGroup.Add(accountProfileGroupTwo);
            await dbContext.SaveChangesAsync();

            //triage
            var triages = EntityCreator.CreateTransactionTriage(
                10,
                profileGroups: new List<Guid> { profileGroupOne.Id, profileGroupTwo.Id });

            //pool
            var txPool = EntityCreator.CreateTransactionPool(triages);
            txPool.First(tp => tp.Code == "Code6").SetLocked(secondaryProfileAccount);
            txPool.First(tp => tp.Code == "Code8").SetLocked(secondaryProfileAccount);

            dbContext.TransactionPools.AddRange(txPool);
            await dbContext.SaveChangesAsync();


            //Act
            var txPendingsCaseOne = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(2, currentProfileAccount);
            var txPendingsCaseTwo = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(5, currentProfileAccount);
            var txPendingsCaseThree = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(7, currentProfileAccount);
            var txPendingsCaseFour = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(7, secondaryProfileAccount);
            var txPendingsCaseFive = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(7, Guid.NewGuid());


            //Assert
            //case one
            Assert.Equal(2, txPendingsCaseOne.Count());
            Assert.Contains(txPendingsCaseOne, tp => tp.Code == "Code1");
            Assert.Contains(txPendingsCaseOne, tp => tp.Code == "Code3");

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
        public async Task SetToPendinglShouldSetRegistryInPendingAsync()
        {
            //Arrange
            var transactionTriages = EntityCreator.CreateTransactionTriage(5);
            var twoTriage = transactionTriages.First(i => i.Code == "Code2");

            var transactionRegistries = EntityCreator.CreateTransactionRegistry(transactionTriages);
            dbContext.TransactionRegistries.AddRange(transactionRegistries);
            dbContext.SaveChanges();


            //Act
            var txRegistry = await transactionGeneratorService.SetToPendingAsync(twoTriage.TrackingIdentify);
            await dbContext.SaveChangesAsync();


            //Assert
            Assert.Equal(TransactionStep.Pending, txRegistry.TransactionStep);
        }
    }
}
