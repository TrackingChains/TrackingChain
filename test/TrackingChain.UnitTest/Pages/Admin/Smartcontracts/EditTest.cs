using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.UnitTest.Helpers;
using Xunit;
using TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts;

namespace TrackingChain.UnitTest.Pages.Admin.Smartcontracts
{
#pragma warning disable CA1001 // Not need in unit test
    public class EditTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;

        public EditTest()
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
        public async Task OnPostShoudleEditSmartContractAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var smartContractToEdit = await dbContext.SmartContracts.FirstAsync();
            var createModel = new EditModel(dbContext);
            var smartContractBinding = new SmartContractBinding
            {
                Id = smartContractToEdit.Id,
                Address = "0x231222324987",
                ChainNumberId = 102,
                ChainType = ChainType.EVM,
                Currency = "ASBEdit",
                ExtraInfo = "{\"ByteWeight\":10}",
                Name = "SmartNameUniqueEdit"
            };
            createModel.SmartContractBinding = smartContractBinding;
            var startingSmartContract = await dbContext.SmartContracts.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingSmartContract, await dbContext.SmartContracts.CountAsync());
            var smartContract = await dbContext.SmartContracts.Where(pg => pg.Name == smartContractBinding.Name).FirstAsync();
            Assert.Equal(smartContractBinding.Address, smartContract.Address);
            Assert.Equal(smartContractBinding.ChainNumberId, smartContract.ChainNumberId);
            Assert.Equal(smartContractBinding.ChainType, smartContract.ChainType);
            Assert.Equal(smartContractBinding.Currency, smartContract.Currency);
            Assert.Equal("{\"ByteWeight\":10,\"GetTrackSelectorValue\":null,\"InsertTrackSelectorValue\":null,\"InsertTrackBasicWeight\":0,\"InsertTrackProofSize\":0,\"InsertTrackRefTime\":0,\"SupportedClient\":0,\"WaitingSecondsForWatcherTx\":0}", smartContract.ExtraInfo);
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
                Id = 101,
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
