using Microsoft.Extensions.Logging;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Web3;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.TransactionGeneratorCore.SmartContracts;

namespace TrackingChain.TransactionGeneratorCore.Services
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
        public Task<TransactionDetail> GetTrasactionReceiptAsync(
            string txHash, 
            string chainEndpoint)
        {
            return Task.FromResult(new TransactionDetail
            {
                ContractAddress = "",
                BlockNumber = "",
                BlockHash = "",
                CumulativeGasUsed = "",
                EffectiveGasPrice = "",
                From = "",
                GasUsed = "",
                Successful = true,
                To = "",
                TransactionHash = ""
            });
        }

        public async Task<string> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token)
        {
            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainEndpoint,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            return await contractHandler.SendRequestAsync(insertTracking);
        }

        public async Task<TransactionDetail> InsertTrackingAndWaitForReceiptAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token)
        {
            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainEndpoint,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            var ethReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(insertTracking);

            return new TransactionDetail
            {
                BlockHash = ethReceipt.BlockHash,
                ContractAddress = contractAddress,
                From = ethReceipt.From,
                To = ethReceipt.To,
                TransactionHash = ethReceipt.TransactionHash
            };
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
            string chainRpc,
            string contractAddress)
        {
            var account = new Nethereum.Web3.Accounts.Account(privateKey, chainNumberId);
            var web3 = new Web3(account, chainRpc);

            return web3.Eth.GetContractHandler(contractAddress);
        }
    }
}
