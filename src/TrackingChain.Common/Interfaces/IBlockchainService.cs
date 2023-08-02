using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;

namespace TrackingChain.Common.Interfaces
{
    public interface IBlockchainService
    {
        Task<string> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token);

        /*Task<TransactionDetail> InsertTrackingAndWaitForReceiptAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token);*/
        ChainType ProviderType { get; }
    }
}
