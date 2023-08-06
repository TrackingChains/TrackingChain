﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Extensions;
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
                    pending.SetLocked(account.Id);
                    await applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    applicationDbContext.Entry(pending).State = EntityState.Unchanged;
                    continue;
                }

                var blockChainService = blockchainServices.First(x => x.ProviderType == pending.ChainType);
                var (apiUrl, apiKey) = account.GetFirstRandomWatcherAddress;
                
                if (!string.IsNullOrWhiteSpace(apiUrl))
                {
                    TransactionDetail? transactionDetail;

                    try
                    {
                        transactionDetail = await blockChainService.GetTrasactionReceiptAsync(pending.TxHash, apiUrl, apiKey);
                    }
#pragma warning disable CA1031 // We need fot catch all problems.
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    { //TODO after some time is null so going in error and need manual check  (MileStone2)

                        logger.GetTrasactionReceiptInError(pending.TrackingId, pending.TxHash, apiUrl, ex);
                        pending.Unlock();
                        await applicationDbContext.SaveChangesAsync();
                        return false;
                    }
                    if (transactionDetail is null)
                    {
                        //TODO after some time is null so going in error and need manual check (MileStone2)
                        pending.Unlock();
                        await applicationDbContext.SaveChangesAsync();
                        break;
                    }

                    if (transactionDetail.Successful.HasValue &&
                        transactionDetail.Successful.Value)
                        await TransactionCompletedAsync(pending, transactionDetail);
                    else
                    {
                        //TODO manage error (Milestone 2)
                    }
                }
                else
                {
                    await TransactionCompletedAsync(pending, new TransactionDetail(true));
                }

                await applicationDbContext.SaveChangesAsync();
            }

            return pendings.Any();
        }

        // Helpers.
        private async Task TransactionCompletedAsync(
            TransactionPending pending, 
            TransactionDetail transactionDetail)
        {
            pending.SetCompleted();
            await transactionWatcherService.SetToRegistryAsync(
                pending.TrackingId,
                transactionDetail);
            await transactionWatcherService.SetTransactionPoolCompletedAsync(pending.TrackingId);
            await transactionWatcherService.SetTransactionTriageCompletedAsync(pending.TrackingId);

            logger.TransactionCompleted(pending.TrackingId, transactionDetail.Successful);
        }
    }
}
