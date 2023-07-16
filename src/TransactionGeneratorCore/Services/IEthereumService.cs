using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace TrackingChain.TransactionGeneratorCore.Services
{
    public interface IEthereumService
    {
        Task<string> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress);

        Task<TransactionReceipt> InsertTrackingAndWaitForReceiptAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress);
    }
}
