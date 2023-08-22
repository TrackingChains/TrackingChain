using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Pages.Admin.ProfileGroups
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
        public async Task OnPostShoudleCreateProfileGroupAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var createModel = new CreateModel(dbContext);
            var aggregationCode = "ChainWriterAddressTest";
            var authority = "ChainWatcherAddressTest";
            var category = "CategoryTest";
            var name = "NameTestUnique";
            var smartContractId = 1;
            var priority = 2;
            var profileGroupBinding = new ProfileGroupBinding
            {
                AggregationCode = aggregationCode,
                Authority = authority,
                Category = category,
                Name = name,
                SmartContractId = smartContractId,
                Priority = priority,
            };
            createModel.ProfileGroupBinding = profileGroupBinding;
            var startingProfileGroup = await dbContext.ProfileGroups.CountAsync();


            //Act
            var result = await createModel.OnPostAsync();


            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            Assert.Equal(startingProfileGroup + 1, await dbContext.ProfileGroups.CountAsync());
            var profileGroup = await dbContext.ProfileGroups.Where(pg => pg.Name == name).FirstAsync();
            Assert.Equal(aggregationCode, profileGroup.AggregationCode);
            Assert.Equal(authority, profileGroup.Authority);
            Assert.Equal(category, profileGroup.Category);
            Assert.Equal(name, profileGroup.Name);
            Assert.Equal(smartContractId, profileGroup.SmartContractId);
            Assert.Equal(priority, profileGroup.Priority);
        }

        [Fact]
        public async Task OnPostShoudleGet404WhenSmartContracNotFoundAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var secondaryProfile = Guid.NewGuid();
            await EntityCreator.CreateConfigurationDatabaseAsync(primaryProfile, secondaryProfile, dbContext);

            var createModel = new CreateModel(dbContext);
            var aggregationCode = "ChainWriterAddressTest";
            var authority = "ChainWatcherAddressTest";
            var category = "CategoryTest";
            var name = "NameTestUnique";
            var smartContractId = 100;
            var priority = 2;
            var profileGroupBinding = new ProfileGroupBinding
            {
                AggregationCode = aggregationCode,
                Authority = authority,
                Category = category,
                Name = name,
                SmartContractId = smartContractId,
                Priority = priority,
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
