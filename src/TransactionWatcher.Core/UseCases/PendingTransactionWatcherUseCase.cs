using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionWatcherCore.Services;

namespace TrackingChain.TransactionWatcherCore.UseCases
{
    public class PendingTransactionWatcherUseCase : IPendingTransactionWatcherUseCase
    {
        // Fields.
        private readonly IAccountService accountService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IEnumerable<IBlockchainService> blockchainServices;
        private readonly ILogger<PendingTransactionWatcherUseCase> logger;
        private readonly ITransactionWatcherService transactionWatcherService;

        // Constructors.
        public PendingTransactionWatcherUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEnumerable<IBlockchainService> blockchainServices,
            ILogger<PendingTransactionWatcherUseCase> logger,
            ITransactionWatcherService transactionWatcherService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.blockchainServices = blockchainServices;
            this.logger = logger;
            this.transactionWatcherService = transactionWatcherService;
        }

        // Methods.
        public async Task<bool> CheckTransactionStatusAsync(
            int max,
            Guid accountId)
        {
            var account = await accountService.GetAccountAsync(accountId);
            var pendings = await transactionWatcherService.GetTransactionToCheckAsync(max, accountId);

            foreach (var pending in pendings)
            {
                try
                {
                    pending.SetLoked(account.Id);
                    await applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    applicationDbContext.Entry(pending).State = EntityState.Unchanged;
                    continue;
                }

                TransactionDetail transactionDetail;
#pragma warning disable CA1031 // Variable is declared but never used
                try
                {
                    if (pending.ChainType == ChainType.EVM)
                    {
                        var blockChainService = blockchainServices.First(x => x.ProviderType == ChainType.EVM);
                        transactionDetail = await blockChainService.GetTrasactionReceiptAsync(pending.TxHash, account.GetFirstRandomRpcAddress);
                    }
                    else
                    {
                        var blockChainService = blockchainServices.First(x => x.ProviderType == ChainType.Substrate);
                        transactionDetail = await blockChainService.GetTrasactionReceiptAsync(pending.TxHash, account.GetFirstRandomWsAddress);
                    }
                }
                catch (Exception ex)
                { //TODO after some time is null so going in error and need manual check
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                    logger.LogError(ex, "Errore in CheckTransactionStatusAsync");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                    pending.Unlock();
                    await applicationDbContext.SaveChangesAsync();
                    return false;
                }
#pragma warning restore CA1031 // Variable is declared but never used
                if (transactionDetail is null)
                {
                    //TODO after some time is null so going in error and need manual check
                    pending.Unlock();
                    await applicationDbContext.SaveChangesAsync();
                    continue;
                }

                if (transactionDetail.Successful)
                {
                    pending.SetCompleted();
                    await transactionWatcherService.SetToRegistryAsync(
                        pending.TrackingId,
                        transactionDetail);
                    await transactionWatcherService.SetTransactionPoolCompletedAsync(pending.TrackingId);
                    await transactionWatcherService.SetTransactionTriageCompletedAsync(pending.TrackingId);
                }
                else
                {
                    //TODO manage error
                }

                await applicationDbContext.SaveChangesAsync();
            }

            return pendings.Any();
        }
    }
}
