using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace TrackingChain.Substrate.Generic.Client.Clients
{
    public interface ISubstrateClient
    {
        Account Account { get; set; }
        ExtrinsicManager ExtrinsicManager { get; }
        bool IsConnected { get; }
        SubstrateClient SubstrateClient { get; }

        Task<bool> ConnectAsync(bool useMetadata, bool standardSubstrate, CancellationToken token);
        Task<string?> ContractsCallAsync(
            bool callSigned,
            IType dest, 
            BigInteger value, 
            ulong refTime, 
            ulong proofSize, 
            BigInteger? storageDepositLimit, 
            byte[] data, 
            CancellationToken token);
        Task<string?> ContractsCallAndWatchAsync(
            IType dest,
            BigInteger value,
            ulong refTime,
            ulong proofSize,
            BigInteger? storageDepositLimit,
            byte[] data,
            string extrinsicType,
            CancellationToken token);
        Task<bool> DisconnectAsync();
    }
}