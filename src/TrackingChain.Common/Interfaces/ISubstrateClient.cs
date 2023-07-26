using TrackingChain.Common.ExtraInfos;

namespace TrackingChain.Common.Interfaces
{
    public interface ISubstrateClient
    {
        Task<string> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainWs,
            string contractAddress,
            SubstractContractExtraInfo substractContractExtraInfo,
            CancellationToken token);
    }
}
