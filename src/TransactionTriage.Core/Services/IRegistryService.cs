using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionTriageCore.Services
{
    public interface IRegistryService
    {
        Task<SmartContract> GetSmartContractAsync(long smartContractId);
    }
}
