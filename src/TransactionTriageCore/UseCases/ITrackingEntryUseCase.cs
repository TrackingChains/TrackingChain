using System.Threading.Tasks;

namespace TrackingChain.TransactionTriageCore.UseCases
{
    public interface ITrackingEntryUseCase
    {
        Task<string> AddTransactionAsync(
            string code, 
            string data, 
            string category);
    }
}
