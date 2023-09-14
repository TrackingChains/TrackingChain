using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using Nethereum.Contracts.ContractHandlers;
using TrackingChain.EVM.Generic.Client.Model;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Linq;
using System;
using Nethereum.RPC.Eth.DTOs;

namespace EVM.Generic.Client
{
    public class NethereumService : IBlockchainService
    {
        // Fields.
        private readonly ILogger<NethereumService> logger;


        // Constractor.
        public NethereumService(ILogger<NethereumService> logger)
        {
            this.logger = logger;
        }

        // Properties.
        public ChainType ProviderType => ChainType.EVM;

        // Public methods.
        public async Task<TrackingChainData?> GetTrasactionDataAsync(
            string code,
            string contractAddress,
            string chainEndpoint,
            int chainNumberId,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token)
        {
            var account = new Account("", chainNumberId);
            var web3 = new Web3(account, chainEndpoint);

            var contractHandler = web3.Eth.GetContractHandler(contractAddress);

            var getProductTrackingFunction = new GetCodeTrackingFunction();
            getProductTrackingFunction.Code = Encoding.ASCII.GetBytes(code);
            var getTrackingResult = await contractHandler.QueryDeserializingToObjectAsync<GetCodeTrackingFunction, GetCodeTrackingOutputDTO>(getProductTrackingFunction);

            return new TrackingChainData
            {
                Code = code,
                DataValues = getTrackingResult?.ReturnValue1?
                    .Select(dv => new DataDetail
                    {
                        BlockNumber = (long)dv.BlockNumber,
                        Timestamp = (long)dv.Timestamp,
                        DataValue = dv.DataValue
                    }) ?? Array.Empty<DataDetail>()
            };
        }

        public async Task<TransactionDetail?> GetTrasactionReceiptAsync(
            string txHash,
            string chainEndpoint,
            string? apiKey)
        {
            var web3 = new Web3(chainEndpoint);
            var ethReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);

            if (ethReceipt is null)
                return null;

            return new TransactionDetail(
                ethReceipt.BlockHash,
                ethReceipt.BlockNumber.HexValue,
                ethReceipt.ContractAddress,
                ethReceipt.CumulativeGasUsed.HexValue,
                ethReceipt.EffectiveGasPrice.HexValue,
                "",
                ethReceipt.From,
                ethReceipt.GasUsed.HexValue,
                ethReceipt.Status.Value == 1,
                ethReceipt.TransactionHash,
                ethReceipt.To);
        }

        public async Task<TransactionDetail?> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(contractExtraInfo);    

            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainEndpoint,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            if (contractExtraInfo.WaitingForResult)
            {
                var receipt = await contractHandler.SendRequestAndWaitForReceiptAsync(insertTracking);
                return ToTransactionDetail(receipt);
            }
            else
            {
                var txHash = await contractHandler.SendRequestAsync(insertTracking);
                return ToTransactionDetail(txHash);
            }
        }

        // Helpers.
        private InsertTrackFunction CommonInsertTracking(
            string code,
            string dataValue)
        {
            return new InsertTrackFunction
            {
                Code = Encoding.ASCII.GetBytes(code),
                DataValue = Encoding.ASCII.GetBytes(dataValue),
                Closed = false
            };
        }

        private ContractHandler GetContractHandler(
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress)
        {
            var account = new Account(privateKey, chainNumberId);
            var web3 = new Web3(account, chainEndpoint);

            return web3.Eth.GetContractHandler(contractAddress);
        }

        public TransactionDetail? ToTransactionDetail(string txHash) => new TransactionDetail(txHash);

        public TransactionDetail? ToTransactionDetail(TransactionReceipt ethReceipt)
        {
            if (ethReceipt is null)
                return null;

            return new TransactionDetail(
                ethReceipt.BlockHash,
                ethReceipt.BlockNumber.HexValue,
                ethReceipt.ContractAddress,
                ethReceipt.CumulativeGasUsed.HexValue,
                ethReceipt.EffectiveGasPrice.HexValue,
                "",
                ethReceipt.From,
                ethReceipt.GasUsed.HexValue,
                ethReceipt.Status.Value == 1,
                ethReceipt.TransactionHash,
                ethReceipt.To);
        }
    }
}