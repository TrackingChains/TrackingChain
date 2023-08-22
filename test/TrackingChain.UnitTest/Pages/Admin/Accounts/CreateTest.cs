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
            var chainWriterAddress = "ChainWriterAddressTest";
            var chainWatcherAddress = "ChainWatcherAddressTest";
            var name = "NameTest";
            var privateKey = "PrivateKeyTest";
            var accountBinding = new AccountBinding
            {
                ChainWriterAddress = chainWriterAddress,
                ChainWatcherAddress = chainWatcherAddress,
                Name = name,
                PrivateKey = privateKey
            };
            createModel.AccountBinding = accountBinding;


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(1, await dbContext.Accounts.CountAsync());
            var account = await dbContext.Accounts.FirstAsync();
            Assert.Equal(chainWriterAddress, account.ChainWriterAddress);
            Assert.Equal(chainWatcherAddress, account.ChainWatcherAddress);
            Assert.Equal(name, account.Name);
            Assert.Equal(privateKey, account.PrivateKey);
        }
    }
}
