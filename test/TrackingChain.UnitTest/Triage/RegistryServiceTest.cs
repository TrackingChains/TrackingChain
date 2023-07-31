using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.Services;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Triage
{
#pragma warning disable CA1001 // Not need in unit test
    public class RegistryServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly RegistryService registryService;
        private readonly ApplicationDbContext dbContext;

        public RegistryServiceTest()
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

            registryService = new RegistryService(
                dbContext,
                Mock.Of<ILogger<RegistryService>>());
        }

        [Fact]
        public async Task GetSmartContractAsync()
        {
            //Arrange
            var selectedSmartContractId = 2;
            var smartContracts = EntityCreator.CreateSmartContract(5); 
            dbContext.SmartContracts.AddRange(smartContracts);
            dbContext.SaveChanges();
            var selectedSmartContract = smartContracts.First(i => i.Id == selectedSmartContractId);


            //Act
            var smartContract = await registryService.GetSmartContractAsync(selectedSmartContractId);


            //Assert
            Assert.Equal(selectedSmartContract.Name, smartContract.Name);
            Assert.Equal(selectedSmartContract.ChainNumberId, smartContract.ChainNumberId);
            Assert.Equal(selectedSmartContract.Currency, smartContract.Currency);
            Assert.Equal(selectedSmartContract.ChainType, smartContract.ChainType);
            Assert.Equal(selectedSmartContract.Address, smartContract.Address);
            Assert.Equal(selectedSmartContract.ExtraInfo, smartContract.ExtraInfo);
        }

    }
}
