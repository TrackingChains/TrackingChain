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
        private readonly TransactionTriageService transactionTriageService;
        private readonly ApplicationDbContext dbContext;

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

            var mockRegistryService = new Mock<IRegistryService>();
            mockRegistryService.Setup(m => m.GetSmartContractAsync(1))
                .Returns(Task.FromResult(EntityCreator.CreateSmartContract(1).First()));

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            dbContext = new ApplicationDbContext(options, mock.Object);

            transactionTriageService = new TransactionTriageService(
                dbContext,
                Mock.Of<ILogger<TransactionTriageService>>(),
                mockRegistryService.Object);
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
