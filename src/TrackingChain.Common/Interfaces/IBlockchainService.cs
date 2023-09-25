using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;

namespace TrackingChain.Common.Interfaces
{
    public interface IBlockchainService
    {
        Task<TrackingChainData?> GetTrasactionDataAsync(
            string code,
            string contractAddress,
            string chainEndpoint,
            int chainNumberId,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token);

        Task<TransactionDetail?> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token);

        Task<TransactionDetail?> GetTrasactionReceiptAsync(
            string txHash,
            string chainEndpoint,
            string? apiKey);

        ChainType ProviderType { get; }
    }
}
