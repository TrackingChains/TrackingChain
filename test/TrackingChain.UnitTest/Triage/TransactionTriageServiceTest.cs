using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.Services;
using TrackingChain.TransactionWaitingCore.Services;
using TrackingChain.UnitTest.Helpers;
using Xunit;

namespace TrackingChain.UnitTest.Triage
{

#pragma warning disable CA1001 // Not need in unit test
    public class TransactionTriageServiceTest
#pragma warning restore CA1001 // Not need in unit test
    {
        private readonly ITransactionTriageService transactionTriageService;
        private readonly ApplicationDbContext dbContext;
        private readonly Mock<IRegistryService> mockRegistryService;

        public TransactionTriageServiceTest()
        {
            var databaseOptions = new DatabaseOptions()
            {
                ConnectionString = "",
                DbType = "InMemory",
                UseMigrationScript = false
            };
            var mock = new Mock<IOptionsSnapshot<DatabaseOptions>>();
            mock.Setup(m => m.Value).Returns(databaseOptions);

            mockRegistryService = new Mock<IRegistryService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            dbContext = new ApplicationDbContext(options, mock.Object);

            transactionTriageService = new TransactionTriageService(
                dbContext,
                Mock.Of<ILogger<TransactionTriageService>>(),
                mockRegistryService.Object);
        }

        [Fact]
        public async Task GetProfileGroupForTransactionShouldFindContractByCategoryNullAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryCategory = "categoryPrime";
            var secondaryProfile = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            await dbContext.SaveChangesAsync();


            //Act
            var profileGroup = await transactionTriageService.GetProfileGroupForTransactionAsync("authTest", "TestCode1", primaryCategory);


            //Assert
            Assert.NotNull(profileGroup);
            Assert.Null(profileGroup.Category);
            Assert.Equal(1, profileGroup.SmartContractId);
        }

        [Fact]
        public async Task GetSmartContractForTransactionShouldFindContractByCategoryNameAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryCategory = "categoryPrime";
            var secondaryProfile = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            var primaryGroup = await dbContext.AccountProfileGroup
                .Where(apg => apg.AccountId == primaryProfile)
                .Include(i => i.ProfileGroup)
                .Select(apg => apg.ProfileGroup)
                .SingleAsync();
            primaryGroup.SetCategory(primaryCategory);
            dbContext.ProfileGroups.Update(primaryGroup);
            await dbContext.SaveChangesAsync();


            //Act
            var profileGroup = await transactionTriageService.GetProfileGroupForTransactionAsync("authTest", "TestCode1", primaryCategory);


            //Assert
            Assert.NotNull(profileGroup);
            Assert.Equal(primaryCategory, profileGroup.Category);
            Assert.Equal(1, profileGroup.SmartContractId);
        }

        [Fact]
        public async Task GetSmartContractForTransactionShouldNotFoundContractByCategoryNameAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryCategory = "categoryPrime";
            var secondaryProfile = Guid.NewGuid();
            var secondaryCategory = "categorySecond";

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            var primaryGroup = await dbContext.AccountProfileGroup
                .Where(apg => apg.AccountId == primaryProfile)
                .Include(i => i.ProfileGroup)
                .Select(apg => apg.ProfileGroup)
                .SingleAsync();
            primaryGroup.SetCategory(primaryCategory);
            dbContext.ProfileGroups.Update(primaryGroup);
            var secondaryGroup = await dbContext.AccountProfileGroup
                .Where(apg => apg.AccountId == secondaryProfile)
                .Include(i => i.ProfileGroup)
                .Select(apg => apg.ProfileGroup)
                .SingleAsync();
            secondaryGroup.SetCategory(secondaryCategory);
            dbContext.ProfileGroups.Update(secondaryGroup);
            await dbContext.SaveChangesAsync();


            //Act
            async Task act() => await transactionTriageService.GetProfileGroupForTransactionAsync("authTest", "TestCode1", "CategoryNameNotFound");


            //Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.Equal("Smart contract group not found", exception.Message);
            Assert.Equal("authTest", exception.Data["Authority"]);
            Assert.Equal("TestCode1", exception.Data["Code"]);
            Assert.Equal("CategoryNameNotFound", exception.Data["Category"]);
        }

        [Fact]
        public async Task GetProfileGroupForTransactionShouldFindContractByAuthorityNullAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryAuth = "authTestPrime";
            var secondaryProfile = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            await dbContext.SaveChangesAsync();


            //Act
            var profileGroup = await transactionTriageService.GetProfileGroupForTransactionAsync(primaryAuth, "TestCode1", "TestCategory1");


            //Assert
            Assert.NotNull(profileGroup);
            Assert.Null(profileGroup.Category);
            Assert.Equal(1, profileGroup.SmartContractId);
        }

        [Fact]
        public async Task GetSmartContractForTransactionShouldFindContractByAuthorityNameAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryAuth = "authTestPrime";
            var secondaryProfile = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            var primaryGroup = await dbContext.AccountProfileGroup
                .Where(apg => apg.AccountId == primaryProfile)
                .Include(i => i.ProfileGroup)
                .Select(apg => apg.ProfileGroup)
                .SingleAsync();
            primaryGroup.SetAuthority(primaryAuth);
            dbContext.ProfileGroups.Update(primaryGroup);
            await dbContext.SaveChangesAsync();


            //Act
            var profileGroup = await transactionTriageService.GetProfileGroupForTransactionAsync(primaryAuth, "TestCode1", "TestCategory1");


            //Assert
            Assert.NotNull(profileGroup);
            Assert.Equal(primaryAuth, profileGroup.Authority);
            Assert.Equal(1, profileGroup.SmartContractId);
        }

        [Fact]
        public async Task GetSmartContractForTransactionShouldNotFoundContractByAuthorityNameAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryAuth = "authTestPrime";
            var secondaryProfile = Guid.NewGuid();
            var secondaryAuth = "authTestSecond";

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            var primaryGroup = await dbContext.AccountProfileGroup
                .Where(apg => apg.AccountId == primaryProfile)
                .Include(i => i.ProfileGroup)
                .Select(apg => apg.ProfileGroup)
                .SingleAsync();
            primaryGroup.SetAuthority(primaryAuth);
            dbContext.ProfileGroups.Update(primaryGroup);
            var secondaryGroup = await dbContext.AccountProfileGroup
                .Where(apg => apg.AccountId == secondaryProfile)
                .Include(i => i.ProfileGroup)
                .Select(apg => apg.ProfileGroup)
                .SingleAsync();
            secondaryGroup.SetAuthority(secondaryAuth);
            dbContext.ProfileGroups.Update(secondaryGroup);
            await dbContext.SaveChangesAsync();


            //Act
            async Task act() => await transactionTriageService.GetProfileGroupForTransactionAsync("authNameNotFound", "TestCode1", "CategoryTEst1");


            //Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.Equal("Smart contract group not found", exception.Message);
            Assert.Equal("authNameNotFound", exception.Data["Authority"]);
            Assert.Equal("TestCode1", exception.Data["Code"]);
            Assert.Equal("CategoryTEst1", exception.Data["Category"]);
        }

        [Fact]
        public async Task AddTransactionAsync()
        {
            //Arrange
            var primaryProfile = Guid.NewGuid();
            var primaryAuth = "authTestPrime";
            var primaryCategory = "categoryTestPrime";
            var primaryCode = "codeTestPrime";
            var primaryDataValue = "dataValueTestPrime";
            var secondaryProfile = Guid.NewGuid();

            await EntityCreator.CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfile, secondaryProfile, dbContext);
            await dbContext.SaveChangesAsync();

            mockRegistryService.Setup(m => m.GetSmartContractAsync(1))
                .Returns(dbContext.SmartContracts.SingleAsync(sc => sc.Id == 1));


            //Act
            var triageResult = await transactionTriageService.AddTransactionAsync(primaryAuth, primaryCode, primaryCategory, primaryDataValue);


            //Assert
            Assert.Equal(primaryCode, triageResult.Code);
            Assert.Equal(primaryDataValue, triageResult.DataValue);
            Assert.Equal(1, triageResult.SmartContractId);
            Assert.NotEqual(Guid.Empty, triageResult.TrackingIdentify);
        }

        [Fact]
        public async Task ShouldGetTriageFirstInAsync()
        {
            //Arrange
            var triages = EntityCreator.CreateTransactionTriage(10, codeFixed: "CodeTest");
            dbContext.TransactionTriages.AddRange(triages);
            await dbContext.SaveChangesAsync();

            var txTriages = triages.Where(tt => tt.Id == 2 ||
                                                tt.Id == 6);
            txTriages.ElementAt(0).SetCompleted();
            txTriages.ElementAt(1).SetCompleted();
            dbContext.TransactionTriages.UpdateRange(txTriages);
            await dbContext.SaveChangesAsync();


            //Act
            var triageResult = await transactionTriageService.GetTransactionReadyForPoolAsync(10);


            //Assert
            Assert.Single(triageResult);
            Assert.Contains(triageResult, tt => tt.Id == 1);
        }

        [Fact]
        public async Task ShouldGetTriageExcludeCompletedAsync()
        {
            //Arrange
            var triages = EntityCreator.CreateTransactionTriage(10, codeFixed: "CodeTest");
            dbContext.TransactionTriages.AddRange(triages);
            await dbContext.SaveChangesAsync();

            var txTriages = triages.Where(tt => tt.Id == 1 ||
                                                tt.Id == 2);
            txTriages.ElementAt(0).SetCompleted();
            txTriages.ElementAt(1).SetCompleted();
            dbContext.TransactionTriages.UpdateRange(txTriages);
            await dbContext.SaveChangesAsync();


            //Act
            var triageResult = await transactionTriageService.GetTransactionReadyForPoolAsync(10);


            //Assert
            Assert.Single(triageResult);
            Assert.Contains(triageResult, tt => tt.Id == 3);
        }

        [Fact]
        public async Task ShouldGetTriageExcludeAllCoeInPoolAsync()
        {
            //Arrange
            var triages = EntityCreator.CreateTransactionTriage(10, codeFixed: "CodeTest");
            dbContext.TransactionTriages.AddRange(triages);
            await dbContext.SaveChangesAsync();

            var txTriages = triages.Where(tt => tt.Id == 1);
            txTriages.ElementAt(0).SetInPool();
            dbContext.TransactionTriages.UpdateRange(txTriages);
            await dbContext.SaveChangesAsync();


            //Act
            var triageResult = await transactionTriageService.GetTransactionReadyForPoolAsync(10);


            //Assert
            Assert.Empty(triageResult);
        }

        [Fact]
        public async Task ShouldGetTriageFirstInEvenWithMultipleCodeAsync()
        {
            //Arrange
            var triagesOne = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestOne");
            dbContext.TransactionTriages.AddRange(triagesOne);
            await dbContext.SaveChangesAsync();
            var triagesTwo = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestTwo");
            dbContext.TransactionTriages.AddRange(triagesTwo);
            await dbContext.SaveChangesAsync();
            var triagesThree = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestThree");
            dbContext.TransactionTriages.AddRange(triagesThree);
            await dbContext.SaveChangesAsync();
            var triagesFour = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestFour");
            dbContext.TransactionTriages.AddRange(triagesFour);
            await dbContext.SaveChangesAsync();


            //Act
            var triageResultOne = await transactionTriageService.GetTransactionReadyForPoolAsync(3);
            var triageResultTwo = await transactionTriageService.GetTransactionReadyForPoolAsync(5);


            //Assert
            // case one
            Assert.Equal(3, triageResultOne.Count);
            Assert.Contains(triagesOne, tt => tt.Id == 1);
            Assert.Contains(triagesTwo, tt => tt.Id == 4);
            Assert.Contains(triagesThree, tt => tt.Id == 7);

            // case two
            Assert.Equal(4, triageResultTwo.Count);
        }

        [Fact]
        public async Task ShouldGetTriageFirstInEvenWithMultipleCodeAndLockedCaseAsync()
        {
            //Arrange
            var triagesOne = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestOne");
            dbContext.TransactionTriages.AddRange(triagesOne);
            await dbContext.SaveChangesAsync();
            triagesOne.First(i => i.Id == 1).SetCompleted();
            dbContext.TransactionTriages.Update(triagesOne.First(i => i.Id == 1));

            var triagesTwo = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestTwo");
            dbContext.TransactionTriages.AddRange(triagesTwo);
            await dbContext.SaveChangesAsync();
            triagesTwo.First(i => i.Id == 4).SetInPool();
            dbContext.TransactionTriages.Update(triagesTwo.First(i => i.Id == 4));

            var triagesThree = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestThree");
            dbContext.TransactionTriages.AddRange(triagesThree);
            await dbContext.SaveChangesAsync();

            var triagesFour = EntityCreator.CreateTransactionTriage(3, codeFixed: "CodeTestFour");
            dbContext.TransactionTriages.AddRange(triagesFour);
            await dbContext.SaveChangesAsync();


            //Act
            var triageResultOne = await transactionTriageService.GetTransactionReadyForPoolAsync(2);
            var triageResultTwo = await transactionTriageService.GetTransactionReadyForPoolAsync(5);


            //Assert
            // case one
            Assert.Equal(2, triageResultOne.Count);
            Assert.Contains(triagesOne, tt => tt.Id == 1);
            Assert.Contains(triagesThree, tt => tt.Id == 7);

            // case two
            Assert.Equal(3, triageResultTwo.Count);
            Assert.Contains(triagesOne, tt => tt.Id == 1);
            Assert.Contains(triagesThree, tt => tt.Id == 7);
            Assert.Contains(triagesFour, tt => tt.Id == 10);
        }
    }
}
