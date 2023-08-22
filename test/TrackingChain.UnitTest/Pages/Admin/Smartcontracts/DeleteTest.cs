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
using TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.Smartcontracts
{

#pragma warning disable CA1001 // Not need in unit test
    public class DeleteTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ApplicationDbContext dbContext;

        public DeleteTest()
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
        public async Task OnPostAsync()
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
                ExtraInfo = "{\"ByteWeight\":110}",
                Name = "SmartNameUnique"
            };
            createModel.SmartContractBinding = smartContractBinding;
            await createModel.OnPostAsync();
            var startingSmartContract = await dbContext.SmartContracts.CountAsync();
            var deleteModel = new DeleteModel(dbContext);


            //Act
            var result = await deleteModel.OnPostAsync(smartContractBinding.Id);


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingSmartContract - 1, await dbContext.ProfileGroups.CountAsync());
            var exist = await dbContext.SmartContracts.AnyAsync(a => a.Id == smartContractBinding.Id);
            Assert.False(exist);
        }
    }
}
