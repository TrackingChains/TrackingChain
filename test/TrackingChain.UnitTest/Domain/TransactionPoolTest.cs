using System;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;
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
            TransactionPool transactionPool = CreateGenericEntity();


            //Act
            transactionPool.SetCompleted();


            //Assert
            Assert.True(transactionPool.Completed);
        }

        [Fact]
        public void SetLokedShouldBeSetData()
        {
            //Arrange
            TransactionPool transactionPool = CreateGenericEntity();
            Guid locker = Guid.NewGuid();


            //Act
            transactionPool.SetLocked(locker);


            //Assert
            Assert.True(transactionPool.Locked);
            Assert.Equal(locker, transactionPool.LockedBy);
            Assert.InRange(transactionPool.LockedDated!.Value, DateTime.UtcNow.AddSeconds(-3), DateTime.UtcNow);
            Assert.Equal(PoolStatus.InProgress, transactionPool.Status);
        }

        [Fact]
        public void UnlockShouldBeCleanData()
        {
            //Arrange
            TransactionPool transactionPool = CreateGenericEntity();
            Guid locker = Guid.NewGuid();
            transactionPool.SetLocked(locker);


            //Act
            transactionPool.Unlock();


            //Assert
            Assert.False(transactionPool.Locked);
            Assert.Null(transactionPool.LockedBy);
            Assert.InRange(transactionPool.GeneratingFrom, DateTime.UtcNow.AddSeconds(4), DateTime.UtcNow.AddSeconds(8));
            Assert.Equal(PoolStatus.WaitingForWorker, transactionPool.Status);
        }

        [Fact]
        public void UnlockShouldBeNotIncreaseErrorTime()
        {
            //Arrange
            var transactionPending = CreateGenericEntity();
            Guid locker = Guid.NewGuid();
            transactionPending.SetLocked(locker);


            //Act
            transactionPending.Unlock();


            //Assert
            Assert.Equal(0, transactionPending.ErrorTimes);
        }

        [Fact]
        public void UnlockFromErrorShouldBeClearData()
        {
            //Arrange
            var transactionPool = CreateGenericEntity();
            Guid locker = Guid.NewGuid();
            transactionPool.SetLocked(locker);


            //Act
            transactionPool.UnlockFromError();


            //Assert
            Assert.False(transactionPool.Locked);
            Assert.Null(transactionPool.LockedBy);
            Assert.InRange(transactionPool.GeneratingFrom, DateTime.UtcNow.AddSeconds(4), DateTime.UtcNow.AddSeconds(8));
            Assert.Equal(PoolStatus.WaitingForWorker, transactionPool.Status);
        }

        [Fact]
        public void UnlockFromErrorShouldBeIncreaseErrorTime()
        {
            //Arrange
            var transactionPool = CreateGenericEntity();
            Guid locker = Guid.NewGuid();
            transactionPool.SetLocked(locker);


            //Act
            transactionPool.UnlockFromError();


            //Assert
            Assert.Equal(1, transactionPool.ErrorTimes);
        }

        [Fact]
        public void UnlockFromErrorShouldBeDelayGeneratingFrom()
        {
            //Arrange
            var transactionPool = CreateGenericEntity();
            Guid locker = Guid.NewGuid();
            transactionPool.SetLocked(locker);
            transactionPool.UnlockFromError();
            transactionPool.SetLocked(locker);
            transactionPool.UnlockFromError();
            transactionPool.SetLocked(locker);


            //Act
            transactionPool.UnlockFromError();


            //Assert
            Assert.True(transactionPool.GeneratingFrom > DateTime.UtcNow.AddSeconds(15));
        }

        [Fact]
        public void SetStatusDoneShouldBe()
        {
            //Arrange
            var transactionPool = CreateGenericEntity();


            //Act
            transactionPool.SetStatusDone();


            //Assert
            Assert.Equal(PoolStatus.Done, transactionPool.Status);
        }

        [Fact]
        public void SetStatusErrorDoneShouldBe()
        {
            //Arrange
            var transactionPool = CreateGenericEntity();


            //Act
            transactionPool.SetStatusError();


            //Assert
            Assert.Equal(PoolStatus.Error, transactionPool.Status);
        }

        // Helpers.
        private static TransactionPool CreateGenericEntity()
        {
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
            return transactionPool;
        }
    }
}
