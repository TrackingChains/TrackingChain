using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TransactionGeneratorCore.Services
{
    public interface IAccountService
    {
        Task<Account> GetAccountAsync(Guid accountId);
    }
}
