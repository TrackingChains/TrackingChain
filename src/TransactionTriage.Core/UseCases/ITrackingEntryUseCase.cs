using System;
using System.Threading.Tasks;

namespace TrackingChain.TransactionTriageCore.UseCases
{
    public interface ITrackingEntryUseCase
    {
        Task<Guid> AddTransactionAsync(
            string authority,
            string code, 
            string data, 
            string category);
    }
}
