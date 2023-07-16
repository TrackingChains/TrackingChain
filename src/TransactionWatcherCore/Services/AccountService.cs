using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TransactionWatcherCore.Services
{
    public class AccountService : IAccountService
    {
        // Fields.
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<AccountService> logger;

        // Constractor.
        public AccountService(
            ApplicationDbContext applicationDbContext,
            ILogger<AccountService> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        // Public methods.
        public async Task<Account> GetAccountAsync(Guid accountId)
        {
            var account = await applicationDbContext.Accounts
                .Where(a => a.Id == accountId)
                .FirstOrDefaultAsync();

            if (account is null)
                throw new InvalidOperationException(""); //TODO manage error

            return account;
        }

        public async Task<Account> GetRandomAccountAsync()
        {
            return await applicationDbContext.Accounts.FirstAsync();
        }
    }
}
