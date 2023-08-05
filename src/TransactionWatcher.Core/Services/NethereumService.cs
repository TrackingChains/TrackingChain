using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;

namespace TrackingChain.TransactionWatcherCore.Services
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
        public async Task<TransactionDetail?> GetTrasactionReceiptAsync(
            string txHash,
            string chainEndpoint,
            string? apiKey)
        {
            var web3 = new Web3(chainEndpoint);
            var ethReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);

            if (ethReceipt is null)
                return null;

            return new TransactionDetail
            {
                ContractAddress = ethReceipt.From,
                BlockNumber = ethReceipt.BlockNumber.HexValue,
                BlockHash = ethReceipt.BlockHash,
                CumulativeGasUsed = ethReceipt.CumulativeGasUsed.HexValue,
                EffectiveGasPrice = ethReceipt.EffectiveGasPrice.HexValue,
                From = ethReceipt.From,
                GasUsed = ethReceipt.GasUsed.HexValue,
                Successful = ethReceipt.Status.Value == 1,
                To = ethReceipt.To,
                TransactionHash = ethReceipt.TransactionHash
            };
        }

        public Task<string> InsertTrackingAsync(string code, string dataValue, string privateKey, int chainNumberId, string chainEndpoint, string contractAddress, ContractExtraInfo contractExtraInfo, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}
