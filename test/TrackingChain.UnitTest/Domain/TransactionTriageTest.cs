using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using Xunit;

namespace TrackingChain.UnitTest.Domain
{
    public class TransactionTriageTest
    {
        [Fact]
        public void TransactionPendingShouldBePopulateCorrectly()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;


            //Act
            var transactionTriage = new TransactionTriage(
                code,
                data,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType);


            //Assert
            Assert.Equal(code, transactionTriage.Code);
            Assert.Equal(data, transactionTriage.DataValue);
            Assert.Equal(profileGroupId, transactionTriage.ProfileGroupId);
            Assert.Equal(smartContractId, transactionTriage.SmartContractId);
            Assert.Equal(smartContractAddress, transactionTriage.SmartContractAddress);
            Assert.Equal(smartContractExtraInfo, transactionTriage.SmartContractExtraInfo);
            Assert.Equal(chainNumberId, transactionTriage.ChainNumberId);
            Assert.Equal(chainType, transactionTriage.ChainType);
        }

        [Fact]
        public void SetCompletedShouldBeSetTrue()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;
            var transactionTriage = new TransactionTriage(
                code,
                data,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType);


            //Act
            transactionTriage.SetCompleted();


            //Assert
            Assert.True(transactionTriage.Completed);
        }

        [Fact]
        public void SetInPoolShouldBeSetTrue()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;
            var transactionTriage = new TransactionTriage(
                code,
                data,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType);


            //Act
            transactionTriage.SetInPool();


            //Assert
            Assert.True(transactionTriage.IsInPool);
        }
    }
}
