using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.TriageWebApplication.Pages.Admin.Accounts;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.Accounts
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
        public async Task OnPostAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var editModel = new EditModel(dbContext);
            var accountBinding = new AccountBinding
            {
                Id = dbContext.Accounts.First().Id,
                ChainWriterAddress = "ChainWriterAddressTestEdit",
                ChainWatcherAddress = "ChainWatcherAddressTestEdit",
                Name = "NameTestEdit",
                PrivateKey = "PrivateKeyTestEdit"
            };
            editModel.AccountBinding = accountBinding;
            var startingAccountBinding = await dbContext.Accounts.CountAsync();


            //Act
            var result = await editModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingAccountBinding, await dbContext.Accounts.CountAsync());
            var account = await dbContext.Accounts.Where(a => a.Id == accountBinding.Id).FirstAsync();
            Assert.Equal(accountBinding.ChainWriterAddress, account.ChainWriterAddress);
            Assert.Equal(accountBinding.ChainWatcherAddress, account.ChainWatcherAddress);
            Assert.Equal(accountBinding.Name, account.Name);
            Assert.Equal(accountBinding.PrivateKey, account.PrivateKey);
        }

        [Fact]
        public async Task OnPostShoudleGet404WhenAccountNotFoundAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var editModel = new EditModel(dbContext);
            var accountBinding = new AccountBinding
            {
                Id = Guid.NewGuid(),
                ChainWriterAddress = "ChainWriterAddressTestEdit",
                ChainWatcherAddress = "ChainWatcherAddressTestEdit",
                Name = "NameTestEdit",
                PrivateKey = "PrivateKeyTestEdit"
            };
            editModel.AccountBinding = accountBinding;
            var startingAccountBinding = await dbContext.Accounts.CountAsync();


            //Act
            var result = await editModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirectToPageResult.StatusCode);
            Assert.Equal(startingAccountBinding, await dbContext.Accounts.CountAsync());
        }
        
    }
}
