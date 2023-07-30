using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionWatcherCore.Services;

namespace TrackingChain.TransactionWatcherCore.UseCases
{
    public class PendingTransactionWatcherUseCase : IPendingTransactionWatcherUseCase
    {
        // Fields.
        private readonly IAccountService accountService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IEthereumService ethereumService;
        private readonly ILogger<PendingTransactionWatcherUseCase> logger;
        private readonly ITransactionWatcherService transactionWatcherService;

        // Constructors.
        public PendingTransactionWatcherUseCase(
            IAccountService accountService,
            ApplicationDbContext applicationDbContext,
            IEthereumService ethereumService,
            ILogger<PendingTransactionWatcherUseCase> logger,
            ITransactionWatcherService transactionWatcherService)
        {
            this.accountService = accountService;
            this.applicationDbContext = applicationDbContext;
            this.ethereumService = ethereumService;
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

                TransactionReceipt receipt;
#pragma warning disable CS0168 // Variable is declared but never used
#pragma warning disable CA1031 // Variable is declared but never used
                try
                {
                    receipt = await ethereumService.GetTrasactionReceiptAsync(pending.TxHash, account.GetFirstRandomRpcAddress);
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
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore CA1031 // Variable is declared but never used
                if (receipt?.Status is null)
                {
                    //TODO after some time is null so going in error and need manual check
                    pending.Unlock();
                    await applicationDbContext.SaveChangesAsync();
                    continue;
                }

                if (receipt.Status.Value == 1)
                {
                    pending.SetCompleted();
                    await transactionWatcherService.SetToRegistryAsync(
                        pending.TrackingId,
                        new Common.Dto.TransactionDetail
                        {
                            BlockHash = receipt.BlockHash,
                            BlockNumber = receipt.BlockNumber.HexValue,
                            ContractAddress = receipt.ContractAddress,
                            CumulativeGasUsed = receipt.CumulativeGasUsed.HexValue,
                            EffectiveGasPrice = receipt.EffectiveGasPrice.HexValue,
                            From = receipt.From,
                            GasUsed = receipt.GasUsed.HexValue,
                            To = receipt.To,
                            TransactionHash = receipt.TransactionHash
                        });
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
