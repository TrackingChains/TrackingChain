using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task OnPostShoudleEditProfileGroupAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var accountProfileGroupToEdit = await dbContext.AccountProfileGroup.FirstAsync();
            var createModel = new EditModel(dbContext);
            var accountProfileGroupBinding = new AccountProfileGroupBinding
            {
                AccountId = accountProfileGroupToEdit.AccountId,
                AccountName = "AccountName",
                ProfileGroupId = accountProfileGroupToEdit.ProfileGroupId,
                ProfileGroupName = "ProfileGroupName",
                Name = "AccountProfileGroupTestUniqueEdit",
                Priority = 3
            };
            createModel.AccountProfileGroupBinding = accountProfileGroupBinding;
            var startingAccountProfileGroup = await dbContext.AccountProfileGroup.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingAccountProfileGroup, await dbContext.AccountProfileGroup.CountAsync());
            var profileGroup = await dbContext.AccountProfileGroup.Where(pg => pg.Name == accountProfileGroupBinding.Name).FirstAsync();
            Assert.Equal(accountProfileGroupBinding.AccountId, profileGroup.AccountId);
            Assert.Equal(accountProfileGroupBinding.ProfileGroupId, profileGroup.ProfileGroupId);
            Assert.Equal(accountProfileGroupBinding.Name, profileGroup.Name);
            Assert.Equal(accountProfileGroupBinding.Priority, profileGroup.Priority);
        }

        [Fact]
        public async Task OnPostShoudleGet404WhenAccountNotFoundAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var accountProfileGroupToEdit = await dbContext.AccountProfileGroup.FirstAsync();
            var createModel = new EditModel(dbContext);
            var accountProfileGroupBinding = new AccountProfileGroupBinding
            {
                AccountId = Guid.NewGuid(),
                AccountName = "NameNotFound",
                ProfileGroupId = accountProfileGroupToEdit.ProfileGroupId,
                ProfileGroupName = "ProfileGroupName",
                Name = "AccountProfileGroupTestUniqueEdit",
                Priority = 3
            };
            createModel.AccountProfileGroupBinding = accountProfileGroupBinding;
            var startingAccountProfileGroup = await dbContext.AccountProfileGroup.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirectToPageResult.StatusCode);
            Assert.Equal(startingAccountProfileGroup, await dbContext.ProfileGroups.CountAsync());
        }

        [Fact]
        public async Task OnPostShoudleGet404WhenProfileNotFoundAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var accountProfileGroupBindingToEdit = await dbContext.AccountProfileGroup.FirstAsync();
            var createModel = new EditModel(dbContext);
            var accountProfileGroupBinding = new AccountProfileGroupBinding
            {
                AccountId = accountProfileGroupBindingToEdit.AccountId,
                AccountName = "AccountName",
                ProfileGroupId = Guid.NewGuid(),
                ProfileGroupName = "NameNotFound",
                Name = "AccountProfileGroupTestUniqueEdit",
                Priority = 3
            };
            createModel.AccountProfileGroupBinding = accountProfileGroupBinding;
            var startingAccountProfileGroup = await dbContext.AccountProfileGroup.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirectToPageResult.StatusCode);
            Assert.Equal(startingAccountProfileGroup, await dbContext.ProfileGroups.CountAsync());
        }
    }
}
