using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionWaitingCore.Services
{
    public interface ITransactionTriageService
    {
        Task<TransactionTriage> AddTransactionAsync(string authority, string code, string category, string data);
        TransactionRegistry AddRegistry(TransactionTriage transactionTriage);
        Task<ProfileGroup> GetProfileGroupForTransactionAsync(string authority, string code, string category);
        Task<List<TransactionTriage>> GetTransactionReadyForPoolAsync(int max = 100);
    }
}
