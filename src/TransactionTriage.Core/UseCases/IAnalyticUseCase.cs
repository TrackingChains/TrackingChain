using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TransactionTriageCore.ModelViews;

namespace TrackingChain.TransactionTriageCore.UseCases
{
    public interface IAnalyticUseCase
    {
        Task<TrackingModelView?> GetTrackingAsync(Guid trackingGuid);
        Task<IEnumerable<TrackingModelView>> GetTrackingFailedsAsync(int size, int page);
        Task<IEnumerable<TrackingModelView>> GetTrackingHistoryAsync(Guid trackingGuid);
        Task<IEnumerable<TrackingModelView>> GetTrackingHistoryAsync(string code, long smartContractId);
        Task<TrackingStatusModelView?> GetTrackingStatusAsync(Guid trackingGuid);
        Task<IEnumerable<TrackingModelView>> GetTrackingSuccessfullyAsync(int size, int page);
        Task<IEnumerable<TrackingModelView>> GetTrackingPendingsAsync(int size, int page);
        Task<IEnumerable<TrackingModelView>> GetTrackingPoolsAsync(int size, int page);
        Task<IEnumerable<TrackingModelView>> GetTrackingTriagesAsync(int size, int page);
        Task<TrackingStatusStatisticModelView> GetTrackingStatusStatisticAsync();
    }
}
