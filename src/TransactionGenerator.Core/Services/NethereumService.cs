using Microsoft.Extensions.Logging;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.TransactionGeneratorCore.SmartContracts;

namespace TrackingChain.TransactionGeneratorCore.Services
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
        public async Task<string> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress)
        {
            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainRpc,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            return await contractHandler.SendRequestAsync(insertTracking);
        }

        // Methods.
        public async Task<TransactionReceipt> InsertTrackingAndWaitForReceiptAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress)
        {
            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainRpc,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            return await contractHandler.SendRequestAndWaitForReceiptAsync(insertTracking);
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
