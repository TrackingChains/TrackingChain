using System;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;
using Xunit;

namespace TrackingChain.UnitTest.Domain
{
    public class TransactionRegistryTest
    {
        [Fact]
        public void TransactionRegistryShouldBePopulateCorrectly()
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
            var transactionRegistry = new TransactionRegistry(
                code,
                data,
                trackingIdentify,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType,
                triageDate);


            //Assert
            Assert.Null(transactionRegistry.LastTransactionHash);
            Assert.Equal(code, transactionRegistry.Code);
            Assert.Equal(data, transactionRegistry.DataValue);
            Assert.Equal(DateTime.MinValue, transactionRegistry.PoolDate);
            Assert.Equal(trackingIdentify, transactionRegistry.TrackingId);
            Assert.Equal(triageDate, transactionRegistry.TriageDate);
            Assert.Equal(profileGroupId, transactionRegistry.ProfileGroupId);
            Assert.Equal(smartContractId, transactionRegistry.SmartContractId);
            Assert.Equal(smartContractAddress, transactionRegistry.SmartContractAddress);
            Assert.Equal(smartContractExtraInfo, transactionRegistry.SmartContractExtraInfo);
            Assert.Equal(chainNumberId, transactionRegistry.ChainNumberId);
            Assert.Equal(chainType, transactionRegistry.ChainType);
        }

        [Fact]
        public void ShouldBeSetToPending()
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
            var transactionRegistry = new TransactionRegistry(
                code,
                data,
                trackingIdentify,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType,
                triageDate);
            var txHash = "0x098765";
            var smartContractEndpoint = "http://endpoint.ext";


            //Act
            transactionRegistry.SetToPending(txHash, smartContractEndpoint);


            //Assert
            Assert.Equal(TransactionStep.Pending, transactionRegistry.TransactionStep);
            Assert.Equal(txHash, transactionRegistry.LastTransactionHash);
            Assert.Equal(smartContractEndpoint, transactionRegistry.SmartContractEndpoint);
        }

        [Fact]
        public void ShouldBeSetToPool()
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
            var transactionRegistry = new TransactionRegistry(
                code,
                data,
                trackingIdentify,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType,
                triageDate);


            //Act
            transactionRegistry.SetToPool();


            //Assert
            Assert.Equal(TransactionStep.Pool, transactionRegistry.TransactionStep);
        }

        [Fact]
        public void SetToRegistryShouldBePopolateReceptData()
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
            var transactionRegistry = new TransactionRegistry(
                code,
                data,
                trackingIdentify,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType,
                triageDate);
            var receiptBlockHash = "receiptBlockHashTest";
            var receiptBlockNumber = "receiptBlockNumberTest";
            var receiptCumulativeGasUsed = "receiptCumulativeGasUsedTest";
            var receiptEffectiveGasPrice = "receiptEffectiveGasPriceTest";
            var receiptFrom = "receiptFromTest";
            var receiptGasUsed = "receiptGasUsedTest";
            var receiptSuccessful = true;
            var receiptTransactionHash = "receiptTransactionHashTest";
            var receiptTo = "receiptToTest";


            //Act
            transactionRegistry.SetToRegistry(
                receiptBlockHash,
                receiptBlockNumber,
                receiptCumulativeGasUsed,
                receiptEffectiveGasPrice,
                receiptFrom,
                receiptGasUsed,
                receiptSuccessful,
                receiptTransactionHash,
                receiptTo);


            //Assert
            Assert.Equal(TransactionStep.Completed, transactionRegistry.TransactionStep);
            Assert.Equal(receiptBlockHash, transactionRegistry.ReceiptBlockHash);
            Assert.Equal(receiptBlockNumber, transactionRegistry.ReceiptBlockNumber);
            Assert.Equal(receiptCumulativeGasUsed, transactionRegistry.ReceiptCumulativeGasUsed);
            Assert.Equal(receiptEffectiveGasPrice, transactionRegistry.ReceiptEffectiveGasPrice);
            Assert.Equal(receiptFrom, transactionRegistry.ReceiptFrom);
            Assert.Equal(receiptGasUsed, transactionRegistry.ReceiptGasUsed);
            Assert.Equal(receiptSuccessful, transactionRegistry.ReceiptSuccessful);
            Assert.Equal(receiptTransactionHash, transactionRegistry.ReceiptTransactionHash);
            Assert.Equal(receiptTo, transactionRegistry.ReceiptTo);
        }

        [Fact]
        public void GetFirstRandomEndpointAddressShouldBeReturnEndpointWhenPopulated()
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
            var transactionRegistry = new TransactionRegistry(
                code,
                data,
                trackingIdentify,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType,
                triageDate);
            var txHash = "0x098765";
            var smartContractEndpoint = "http://endpoint.one.ext;http://endpoint.two.ext;http://endpoint.three.ext;";
            transactionRegistry.SetToPending(txHash, smartContractEndpoint);


            //Act
            var endpointRandomOne = transactionRegistry.GetFirstRandomEndpointAddress;
            var endpointRandomTwo = transactionRegistry.GetFirstRandomEndpointAddress;
            var endpointRandomThree = transactionRegistry.GetFirstRandomEndpointAddress;


            //Assert
            Assert.NotNull(endpointRandomOne);
            Assert.Contains(endpointRandomOne, smartContractEndpoint, StringComparison.InvariantCulture);
            Assert.NotNull(endpointRandomTwo);
            Assert.Contains(endpointRandomTwo, smartContractEndpoint, StringComparison.InvariantCulture);
            Assert.NotNull(endpointRandomThree);
            Assert.Contains(endpointRandomThree, smartContractEndpoint, StringComparison.InvariantCulture);
        }

        [Fact]
        public void GetFirstRandomEndpointAddressShouldBeReturnNullWhenEmpty()
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
            var transactionRegistry = new TransactionRegistry(
                code,
                data,
                trackingIdentify,
                smartContractId,
                smartContractAddress,
                smartContractExtraInfo,
                profileGroupId,
                chainNumberId,
                chainType,
                triageDate);


            //Act
            var endpointRandomOne = transactionRegistry.GetFirstRandomEndpointAddress;
            var endpointRandomTwo = transactionRegistry.GetFirstRandomEndpointAddress;
            var endpointRandomThree = transactionRegistry.GetFirstRandomEndpointAddress;


            //Assert
            Assert.Null(endpointRandomOne);
            Assert.Null(endpointRandomTwo);
            Assert.Null(endpointRandomThree);
        }
    }
}
