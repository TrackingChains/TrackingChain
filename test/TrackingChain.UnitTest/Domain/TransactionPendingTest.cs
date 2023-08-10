using System;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using Xunit;

namespace TrackingChain.UnitTest.Domain
{
    public class TransactionPendingTest
    {
        [Fact]
        public void TransactionPendingShouldBePopulateCorrectly()
        {
            //Arrange
            string txHash = "0x1234567890";
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
            var forceWatchingFrom = new DateTime(1987, 7, 23, 12, 15, 0, 0);


            //Act
            var transactionPending = new TransactionPending(
                txHash,
                code,
                data,
                poolDate,
                trackingIdentify, 
                triageDate,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType,
                forceWatchingFrom);


            //Assert
            Assert.Equal(txHash, transactionPending.TxHash);
            Assert.Equal(code, transactionPending.Code);
            Assert.Equal(data, transactionPending.DataValue);
            Assert.Equal(poolDate, transactionPending.PoolDate);
            Assert.Equal(trackingIdentify, transactionPending.TrackingId);
            Assert.Equal(triageDate, transactionPending.TriageDate);
            Assert.Equal(profileGroupId, transactionPending.ProfileGroupId);
            Assert.Equal(smartContractId, transactionPending.SmartContractId);
            Assert.Equal(smartContractAddress, transactionPending.SmartContractAddress);
            Assert.Equal(smartContractExtraInfo, transactionPending.SmartContractExtraInfo);
            Assert.Equal(chainNumberId, transactionPending.ChainNumberId);
            Assert.Equal(chainType, transactionPending.ChainType);
            Assert.Equal(forceWatchingFrom, transactionPending.WatchingFrom);
        }

        [Fact]
        public void SetCompletedShouldBeSetTrue()
        {
            //Arrange
            string txHash = "0x1234567890";
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
            var forceWatchingFrom = new DateTime(1987, 7, 23, 12, 15, 0, 0);
            var transactionPending = new TransactionPending(
                txHash,
                code,
                data,
                poolDate,
                trackingIdentify,
                triageDate,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType,
                forceWatchingFrom);


            //Act
            transactionPending.SetCompleted();


            //Assert
            Assert.True(transactionPending.Completed);
        }

        [Fact]
        public void SetLockedShouldBeSetData()
        {
            //Arrange
            string txHash = "0x1234567890";
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
            var forceWatchingFrom = new DateTime(1987, 7, 23, 12, 15, 0, 0);
            var transactionPending = new TransactionPending(
                txHash,
                code,
                data,
                poolDate,
                trackingIdentify,
                triageDate,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType,
                forceWatchingFrom);
            Guid locker = Guid.NewGuid();


            //Act
            transactionPending.SetLocked(locker);


            //Assert
            Assert.True(transactionPending.Locked);
            Assert.Equal(locker, transactionPending.LockedBy);
            Assert.InRange(transactionPending.LockedDated!.Value, DateTime.UtcNow.AddSeconds(-3), DateTime.UtcNow);
        }

        [Fact]
        public void UnlockShouldBeCleanData()
        {
            //Arrange
            string txHash = "0x1234567890";
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
            var forceWatchingFrom = new DateTime(1987, 7, 23, 12, 15, 0, 0);
            var transactionPending = new TransactionPending(
                txHash,
                code,
                data,
                poolDate,
                trackingIdentify,
                triageDate,
                profileGroupId,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                chainNumberId,
                chainType,
                forceWatchingFrom);
            Guid locker = Guid.NewGuid();
            transactionPending.SetLocked(locker);


            //Act
            transactionPending.Unlock();


            //Assert
            Assert.False(transactionPending.Locked);
            Assert.Null(transactionPending.LockedBy);
            Assert.InRange(transactionPending.WatchingFrom, DateTime.UtcNow.AddSeconds(4), DateTime.UtcNow.AddSeconds(8));
        }
    }
}
