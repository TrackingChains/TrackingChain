using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var poolDate = new DateTime(1987, 7, 23, 03, 15, 0, 0);
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


            //Act
            transactionRegistry.SetToPending(txHash);


            //Assert
            Assert.Equal(TransactionStep.Pending, transactionRegistry.TransactionStep);
            Assert.Equal(txHash, transactionRegistry.LastTransactionHash);
        }

        [Fact]
        public void ShouldBeSetToPool()
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
            var poolDate = new DateTime(1987, 7, 23, 03, 15, 0, 0);
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
            Assert.Equal(receiptTo, transactionRegistry.ReceiptTo);
        }
    }
}
