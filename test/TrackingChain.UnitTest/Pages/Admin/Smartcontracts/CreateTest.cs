using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.Smartcontracts
{
#pragma warning disable CA1001 // Not need in unit test
    public class CreateTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;

        public CreateTest()
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
        }

        [Fact]
        public async Task OnPostShoudleCreateSmartContractAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var createModel = new CreateModel(dbContext);
            var smartContractBinding = new SmartContractBinding
            {
                Address = "0x231222324",
                ChainNumberId = 101,
                ChainType = ChainType.Substrate,
                Currency = "ASB",
                ExtraInfo = "{}",
                Name = "SmartNameUnique"
            };
            createModel.SmartContractBinding = smartContractBinding;
            var startingSmartContract = await dbContext.SmartContracts.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingSmartContract + 1, await dbContext.SmartContracts.CountAsync());
            var smartContract = await dbContext.SmartContracts.Where(pg => pg.Name == smartContractBinding.Name).FirstAsync();
            Assert.Equal(smartContractBinding.Address, smartContract.Address);
            Assert.Equal(smartContractBinding.ChainNumberId, smartContract.ChainNumberId);
            Assert.Equal(smartContractBinding.ChainType, smartContract.ChainType);
            Assert.Equal(smartContractBinding.Currency, smartContract.Currency);
            Assert.Equal("{\"ByteWeight\":0,\"GetTrackSelectorValue\":null,\"InsertTrackSelectorValue\":null,\"InsertTrackBasicWeight\":0,\"InsertTrackProofSize\":0,\"InsertTrackRefTime\":0,\"SupportedClient\":0,\"WaitingSecondsForWatcherTx\":0}", smartContract.ExtraInfo);
            Assert.Equal(smartContractBinding.Name, smartContract.Name);
        }

        [Fact]
        public async Task OnPostShoudleGetErrorForJsonInvalidAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var createModel = new CreateModel(dbContext);
            var smartContractBinding = new SmartContractBinding
            {
                Address = "0x231222324",
                ChainNumberId = 101,
                ChainType = ChainType.Substrate,
                Currency = "ASB",
                ExtraInfo = "{brokenJson}",
                Name = "SmartNameUnique"
            };
            createModel.SmartContractBinding = smartContractBinding;
            var startingSmartContract = await dbContext.SmartContracts.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(startingSmartContract, await dbContext.SmartContracts.CountAsync());
            Assert.Equal("Contract ExtraInfo json not valid", createModel.ErrorMessage);
        }
    }
}
