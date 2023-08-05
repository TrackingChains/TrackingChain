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

        public Task<string> InsertTrackingAsync(string code, string dataValue, string privateKey, int chainNumberId, string chainEndpoint, string contractAddress, ContractExtraInfo contractExtraInfo, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}
