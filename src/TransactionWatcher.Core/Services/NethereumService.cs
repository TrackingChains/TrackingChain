using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Threading.Tasks;

namespace TrackingChain.TransactionWatcherCore.Services
{
    public class NethereumService : IEthereumService
    {
        // Fields.
        private readonly ILogger<NethereumService> logger;

        // Constractor.
        public NethereumService(ILogger<NethereumService> logger)
        {
            this.logger = logger;
        }

        // Public methods.
        public async Task<TransactionReceipt> GetTrasactionReceiptAsync(
            string hashId,
            string chainRpc)
        {
            var web3 = new Web3(chainRpc);
            return await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(hashId);
        }

        // Helpers.
    }
}
