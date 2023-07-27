using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.Core;
using TrackingChain.TrackingChainCore.Domain.Enums;
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
        private readonly ISubstrateClientFactory substrateClientFactory;
        private readonly ITransactionGeneratorService transactionGeneratorService;

        // Constructors.
        public PoolDequeuerUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEthereumService ethereumService,
            ILogger<PoolDequeuerUseCase> logger,
            ISubstrateClientFactory substrateClientFactory,
            ITransactionGeneratorService transactionGeneratorService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.ethereumService = ethereumService;
            this.logger = logger;
            this.substrateClientFactory = substrateClientFactory;
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

                string txHash;
                if (pool.ChainType == ChainType.EVM)
                    txHash = await ethereumService.InsertTrackingAsync(
                        pool.Code,
                        pool.DataValue,
                        account.PrivateKey,
                        pool.ChainNumberId,
                        account.GetFirstRandomAvaiableRpcAddress,
                        pool.SmartContractAddress);
                else
                    txHash = await substrateClientFactory.InsertTrackingAsync(
                            pool.Code,
                            pool.DataValue,
                            account.PrivateKey,
                            pool.ChainNumberId,
                            account.GetFirstRandomWsAddress,
                            pool.SmartContractAddress,
                            SubstractContractExtraInfo.FromJson(pool.SmartContractExtraInfo),
                            CancellationToken.None);

                var txPending = transactionGeneratorService.AddTransactionPendingFroomPool(pool, txHash);
                await transactionGeneratorService.SetToPendingAsync(txPending);

                await applicationDbContext.SaveChangesAsync();
            }

            return pools.Any();
        }
    }
}
