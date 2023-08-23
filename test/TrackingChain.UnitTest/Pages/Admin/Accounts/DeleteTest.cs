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
            var accountBinding = new AccountBinding
            {
                Id = dbContext.Accounts.First().Id,
                ChainWriterAddress = "ChainWriterAddressTestEdit",
                ChainWatcherAddress = "ChainWatcherAddressTestEdit",
                Name = "NameTestEdit",
                PrivateKey = "PrivateKeyTestEdit"
            };
            createModel.AccountBinding = accountBinding;
            await createModel.OnPostAsync();
            var startingAccountBinding = await dbContext.Accounts.CountAsync();
            var deleteModel = new DeleteModel(dbContext);


            //Act
            var result = await deleteModel.OnPostAsync(accountBinding.Id);


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingAccountBinding - 1, await dbContext.Accounts.CountAsync());
            var exist = await dbContext.Accounts.AnyAsync(a => a.Id == accountBinding.Id);
            Assert.False(exist);
        }

    }
}
