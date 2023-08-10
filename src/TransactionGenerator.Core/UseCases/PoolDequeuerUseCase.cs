using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
using TrackingChain.TransactionGeneratorCore.Services;

namespace TrackingChain.TransactionGeneratorCore.UseCases
{
    public class PoolDequeuerUseCase : IPoolDequeuerUseCase
    {
        // Fields.
        private readonly IAccountService accountService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IEnumerable<IBlockchainService> blockchainServices;
        private readonly ILogger<PoolDequeuerUseCase> logger;
        private readonly ITransactionGeneratorService transactionGeneratorService;

        // Constructors.
        public PoolDequeuerUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEnumerable<IBlockchainService> blockchainServices,
            ILogger<PoolDequeuerUseCase> logger,
            ITransactionGeneratorService transactionGeneratorService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.blockchainServices = blockchainServices;
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

                var blockChainService = blockchainServices.First(x => x.ProviderType == pool.ChainType);
                var txHash = await blockChainService.InsertTrackingAsync(
                    pool.Code,
                    pool.DataValue,
                    account.PrivateKey,
                    pool.ChainNumberId,
                    account.ChainWriterAddress,
                    pool.SmartContractAddress,
                    ContractExtraInfo.FromJson(pool.SmartContractExtraInfo),
                    CancellationToken.None);

                var txPending = transactionGeneratorService.AddTransactionPendingFromPool(pool, txHash);
                await transactionGeneratorService.SetToPendingAsync(txPending.TrackingId, txHash);
                await applicationDbContext.SaveChangesAsync();

                logger.TransactionOnChain(txPending.TrackingId, txPending.TxHash, txPending.SmartContractAddress);
            }

            return pools.Any();
        }
    }
}
