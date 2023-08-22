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
using TrackingChain.TriageWebApplication.Pages.Admin.AccountProfileGroups;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.AccountProfileGroups
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
        public async Task OnPostShoudleCreateProfileGroupAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var createModel = new CreateModel(dbContext);
            var accountProfileGroupBinding = new AccountProfileGroupBinding
            {
                AccountId = dbContext.Accounts.First().Id,
                AccountName = dbContext.Accounts.First().Name,
                ProfileGroupId = dbContext.ProfileGroups.First().Id,
                ProfileGroupName = dbContext.ProfileGroups.First().Name,
                Name = "AccountProfileGroupTestUnique",
                Priority = 2
            };
            createModel.AccountProfileGroupBinding = accountProfileGroupBinding;
            await createModel.OnPostAsync();
            var startingAccountProfileGroup = await dbContext.AccountProfileGroup.CountAsync();
            var deleteModel = new DeleteModel(dbContext);


            //Act
            var result = await deleteModel.OnPostAsync(accountProfileGroupBinding.AccountId, accountProfileGroupBinding.ProfileGroupId);


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingAccountProfileGroup - 1, await dbContext.AccountProfileGroup.CountAsync());
            var exist = await dbContext.AccountProfileGroup.AnyAsync(pg => pg.AccountId == accountProfileGroupBinding.AccountId &&
                                                         pg.ProfileGroupId == accountProfileGroupBinding.ProfileGroupId);
            Assert.False(exist);
        }

    }
}
