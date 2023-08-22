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
using TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.ProfileGroups
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

            var profileGroupToEdit = await dbContext.ProfileGroups.FirstAsync();
            var createModel = new EditModel(dbContext);
            var profileGroupBinding = new ProfileGroupBinding
            {
                Id = profileGroupToEdit.Id,
                AggregationCode = "AggregationCodeTestEdit",
                Authority = "AuthorityTestEdit",
                Category = "CategoryTestEdit",
                Name = "NameTestUniqueEdit",
                SmartContractId = 2,
                Priority = 3,
            };
            createModel.ProfileGroupBinding = profileGroupBinding;
            var startingProfileGroup = await dbContext.ProfileGroups.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingProfileGroup, await dbContext.ProfileGroups.CountAsync());
            var profileGroup = await dbContext.ProfileGroups.Where(pg => pg.Name == profileGroupBinding.Name).FirstAsync();
            Assert.Equal(profileGroupBinding.AggregationCode, profileGroup.AggregationCode);
            Assert.Equal(profileGroupBinding.Authority, profileGroup.Authority);
            Assert.Equal(profileGroupBinding.Category, profileGroup.Category);
            Assert.Equal(profileGroupBinding.Name, profileGroup.Name);
            Assert.Equal(profileGroupBinding.SmartContractId, profileGroup.SmartContractId);
            Assert.Equal(profileGroupBinding.Priority, profileGroup.Priority);
        }

        [Fact]
        public async Task OnPostShoudleGet404WhenSmartContracNotFoundAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var profileGroupToEdit = await dbContext.ProfileGroups.FirstAsync();
            var createModel = new EditModel(dbContext);
            var profileGroupBinding = new ProfileGroupBinding
            {
                Id = profileGroupToEdit.Id,
                AggregationCode = "ChainWriterAddressTest",
                Authority = "ChainWatcherAddressTest",
                Category = "CategoryTest",
                Name = "NameTestUnique",
                SmartContractId = 100,
                Priority = 2,
            };
            createModel.ProfileGroupBinding = profileGroupBinding;
            var startingProfileGroup = await dbContext.ProfileGroups.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirectToPageResult.StatusCode);
            Assert.Equal(startingProfileGroup, await dbContext.ProfileGroups.CountAsync());
        }
    }
}
