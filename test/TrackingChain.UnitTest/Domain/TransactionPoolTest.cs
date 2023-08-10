using System;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using Xunit;

namespace TrackingChain.UnitTest.Domain
{
    public class TransactionPoolTest
    {
        [Fact]
        public void TransactionPoolShouldBePopulateCorrectly()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var trackingIdentify = Guid.NewGuid();
            var triageDate = new DateTime(1987, 7, 23, 02, 15, 0, 0);
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;


            //Act
            var transactionPool = new TransactionPool(
                code,
                data,
                trackingIdentify,
                triageDate,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType);


            //Assert
            Assert.Equal(code, transactionPool.Code);
            Assert.Equal(data, transactionPool.DataValue);
            Assert.Equal(trackingIdentify, transactionPool.TrackingId);
            Assert.Equal(triageDate, transactionPool.TriageDate);
            Assert.Equal(profileGroupId, transactionPool.ProfileGroupId);
            Assert.Equal(smartContractId, transactionPool.SmartContractId);
            Assert.Equal(smartContractAddress, transactionPool.SmartContractAddress);
            Assert.Equal(smartContractExtraInfo, transactionPool.SmartContractExtraInfo);
            Assert.Equal(chainNumberId, transactionPool.ChainNumberId);
            Assert.Equal(chainType, transactionPool.ChainType);
        }

        [Fact]
        public void SetCompletedShouldBeSetTrue()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var trackingIdentify = Guid.NewGuid();
            var triageDate = new DateTime(1987, 7, 23, 02, 15, 0, 0);
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;
            var transactionPool = new TransactionPool(
                code,
                data,
                trackingIdentify,
                triageDate,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType);


            //Act
            transactionPool.SetCompleted();


            //Assert
            Assert.True(transactionPool.Completed);
        }

        [Fact]
        public void SetLokedShouldBeSetData()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var poolDate = new DateTime(1987, 7, 23, 03, 15, 0, 0);
            var trackingIdentify = Guid.NewGuid();
            var triageDate = new DateTime(1987, 7, 23, 02, 15, 0, 0);
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;
            var transactionPool = new TransactionPool(
                code,
                data,
                trackingIdentify,
                triageDate,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType);
            Guid locker = Guid.NewGuid();


            //Act
            transactionPool.SetLocked(locker);


            //Assert
            Assert.True(transactionPool.Locked);
            Assert.Equal(locker, transactionPool.LockedBy);
            Assert.InRange(transactionPool.LockedDated!.Value, DateTime.UtcNow.AddSeconds(-3), DateTime.UtcNow);
        }

        [Fact]
        public void UnlockShouldBeCleanData()
        {
            //Arrange
            string code = "CodeTest";
            string data = "DataTest";
            var poolDate = new DateTime(1987, 7, 23, 03, 15, 0, 0);
            var trackingIdentify = Guid.NewGuid();
            var triageDate = new DateTime(1987, 7, 23, 02, 15, 0, 0);
            var profileGroupId = Guid.NewGuid();
            var smartContractId = 10;
            var smartContractAddress = "0x1234";
            var smartContractExtraInfo = "{}";
            var chainNumberId = 1001;
            var chainType = ChainType.Substrate;
            var transactionPool = new TransactionPool(
                code,
                data,
                trackingIdentify,
                triageDate,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType);
            Guid locker = Guid.NewGuid();
            transactionPool.SetLocked(locker);


            //Act
            transactionPool.Unlock();


            //Assert
            Assert.False(transactionPool.Locked);
            Assert.Null(transactionPool.LockedBy);
        }
    }
}
