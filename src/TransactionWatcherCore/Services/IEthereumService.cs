using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace TrackingChain.TransactionWatcherCore.Services
{
    public interface IEthereumService
    {
        Task<TransactionReceipt> GetTrasactionReceiptAsync(string hashId, string chainRpc);
    }
}
