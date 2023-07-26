using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionGeneratorCore.Services;

namespace TrackingChain.TransactionGeneratorCore.UseCases
{
    public class PoolDequeuerUseCase : IPoolDequeuerUseCase
    {
        // Fields.
        private readonly IAccountService accountService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IEthereumService ethereumService;
        private readonly ILogger<PoolDequeuerUseCase> logger;
        private readonly ITransactionGeneratorService transactionGeneratorService;

        // Constructors.
        public PoolDequeuerUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEthereumService ethereumService,
            ILogger<PoolDequeuerUseCase> logger,
            ITransactionGeneratorService transactionGeneratorService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.ethereumService = ethereumService;
            this.logger = logger;
            this.transactionGeneratorService = transactionGeneratorService;
        }

        // Methods.
        public async Task<bool> DequeueTransactionAsync(
            int max, 
            Guid accountId)
        {
            var account = await accountService.GetAccountAsync(accountId);
            var pools = await transactionGeneratorService.GetAvaiableTransactionPoolAsync(max, accountId);

            foreach (var pool in pools)
            {
                try
                {
                    pool.SetLocked(accountId);
                    await applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    applicationDbContext.Entry(pool).State = EntityState.Unchanged;
                    continue;
                }

                var txHash = await ethereumService.InsertTrackingAsync(
                    pool.Code,
                    pool.Data,
                    account.PrivateKey,
                    pool.ChainNumberId,
                    account.GetFirstRandomAvaiableRpcAddress,
                    pool.SmartContractAddress);

                var txPending = transactionGeneratorService.AddTransactionPendingFroomPool(pool, txHash);
                await transactionGeneratorService.SetToPendingAsync(txPending);

                await applicationDbContext.SaveChangesAsync();
            }

            return pools.Any();
        }
    }
}
