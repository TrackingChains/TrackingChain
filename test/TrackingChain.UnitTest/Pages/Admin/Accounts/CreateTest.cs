using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.TriageWebApplication.Pages.Admin.Accounts;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.Accounts
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
        public async Task OnPostAsync()
        {
            //Arrange
            var createModel = new CreateModel(dbContext);
            var accountBinding = new AccountBinding
            {
                ChainWriterAddress = "ChainWriterAddressTest",
                ChainWatcherAddress = "ChainWatcherAddressTest",
                Name = "NameTest",
                PrivateKey = "PrivateKeyTest"
            };
            createModel.AccountBinding = accountBinding;


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(1, await dbContext.Accounts.CountAsync());
            var account = await dbContext.Accounts.FirstAsync();
            Assert.Equal(accountBinding.ChainWriterAddress, account.ChainWriterAddress);
            Assert.Equal(accountBinding.ChainWatcherAddress, account.ChainWatcherAddress);
            Assert.Equal(accountBinding.Name, account.Name);
            Assert.Equal(accountBinding.PrivateKey, account.PrivateKey);
        }
    }
}
