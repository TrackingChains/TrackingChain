using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.UnitTest.Helpers
{
    internal static class EntityCreator
    {
        private static readonly Random random = new();

        public static TransactionRegistry ConvertToRegistry(TransactionPending transactionPending)
        {
            return new TransactionRegistry(
                transactionPending.Code,
                transactionPending.DataValue,
                transactionPending.TrackingId,
                transactionPending.SmartContractId,
                transactionPending.SmartContractAddress,
                transactionPending.SmartContractExtraInfo,
                transactionPending.ProfileGroupId,
                transactionPending.ChainNumberId,
                transactionPending.ChainType,
                transactionPending.TriageDate);
        }

        public static TransactionPending ConvertToPending(TransactionPool transactionPool, string txHash)
        {
            return new TransactionPending(
                txHash,
                transactionPool.Code,
                transactionPool.DataValue,
                transactionPool.ReceivedDate,
                transactionPool.TrackingId,
                transactionPool.TriageDate,
                transactionPool.ProfileGroupId,
                transactionPool.SmartContractId,
                transactionPool.SmartContractAddress,
                transactionPool.SmartContractExtraInfo,
                transactionPool.ChainNumberId,
                transactionPool.ChainType);
        }

        public static IEnumerable<TransactionTriage> CreateTransactionTriage(
            int size,
            List<Guid>? profileGroups = null,
            string? codeFixed = null)
        {
            var transactionTriages = new List<TransactionTriage>();
            if (size == 0)
                return transactionTriages;

            for (int i = 1; i <= size; i++)
            {
                var code = codeFixed is null ? $"Code{i}" : codeFixed;
                var dataValue = $"dataValue{i}";
                var profileGroup = profileGroups != null ? profileGroups[(i - 1) % profileGroups.Count] : Guid.NewGuid();
                var smartContractId = i;
                var smartContractAddress = $"0x{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}";
                var smartContractExtraInfo = $"{{}}";
                var smartContractChainNumber = i * 100;
                var smartContractChainType = i % 2 == 0 ? ChainType.Substrate : ChainType.EVM;
                transactionTriages.Add(new TransactionTriage(
                    code,
                    dataValue,
                    profileGroup,
                    smartContractId,
                    smartContractAddress,
                    smartContractExtraInfo,
                    smartContractChainNumber,
                    smartContractChainType));
            }

            return transactionTriages;
        }

        public static IEnumerable<TransactionPool> CreateTransactionPool(IEnumerable<TransactionTriage> transactionTriages)
        {
            var transactionPools = new List<TransactionPool>();

            foreach (var item in transactionTriages)
                transactionPools.Add(new TransactionPool(
                    item.Code,
                    item.DataValue,
                    item.TrackingIdentify,
                    item.ReceivedDate,
                    item.SmartContractId,
                    item.SmartContractAddress,
                    item.SmartContractExtraInfo,
                    item.ProfileGroupId,
                    item.ChainNumberId,
                    item.ChainType));

            return transactionPools;
        }

        public static IEnumerable<TransactionPending> CreateTransactionPending(
            IEnumerable<TransactionTriage> transactionTriages, 
            DateTime? forceWatchingFrom = null)
        {
            var transactionPendings = new List<TransactionPending>();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            foreach (var item in transactionTriages)
#pragma warning disable CA5394 // No need secure number for test
                transactionPendings.Add(new TransactionPending(
                    new string(Enumerable.Repeat(chars, 32).Select(s => s[random.Next(s.Length)]).ToArray()),
                    item.Code,
                    item.DataValue,
                    DateTime.UtcNow.AddMinutes(-30),
                    item.TrackingIdentify,
                    item.ReceivedDate,
                    item.ProfileGroupId,
                    item.SmartContractId,
                    item.SmartContractAddress,
                    item.SmartContractExtraInfo,
                    item.ChainNumberId,
                    item.ChainType,
                    forceWatchingFrom));
#pragma warning restore CA5394 // No need secure number for test

            return transactionPendings;
        }

        public static IEnumerable<TransactionRegistry> CreateTransactionRegistry(int size)
        {
            var triages = CreateTransactionTriage(size);
            return CreateTransactionRegistry(triages);
        }

        public static IEnumerable<TransactionRegistry> CreateTransactionRegistry(IEnumerable<TransactionTriage> transactionTriages)
        {
            var transactionRegistries = new List<TransactionRegistry>();

            foreach (var item in transactionTriages)
                transactionRegistries.Add(new TransactionRegistry(
                    item.Code,
                    item.DataValue,
                    item.TrackingIdentify,
                    item.SmartContractId,
                    item.SmartContractAddress,
                    item.SmartContractExtraInfo,
                    item.ProfileGroupId,
                    item.ChainNumberId,
                    item.ChainType,
                    item.ReceivedDate));

            return transactionRegistries;
        }

        public static IEnumerable<SmartContract> CreateSmartContract(int size)
        {
            var smartContracts = new List<SmartContract>();
            if (size == 0)
                return smartContracts;

            for (int i = 1; i <= size; i++)
            {
                var smartContractAddress = $"0x{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}";
                var smartContractChainNumber = i * 100;
                var smartContractChainType = i % 2 == 0 ? ChainType.Substrate : ChainType.EVM;
                smartContracts.Add(new SmartContract(
                    smartContractAddress,
                    smartContractChainNumber,
                    smartContractChainType,
                    $"Curr{i}",
                    $"name{i}",
                    new ContractExtraInfo
                    {
                        InsertTrackBasicWeight = 1,
                        ByteWeight = 2,
                        InsertTrackSelectorValue = "aaa",
                        SupportedClient = i % 2 == 0 ? SupportedClient.Shibuya : SupportedClient.Shibuya
                    }));
            }

            return smartContracts;
        }

        public static async Task CreateConfigurationDatabaseAsync(
            Guid primaryProfileAccount,
            Guid secondaryProfileAccount,
            ApplicationDbContext dbContext)
        {
            await CreateFullDatabaseWithProfileAndTriageAsync(0, primaryProfileAccount, secondaryProfileAccount, dbContext);
        }

        public static async Task CreateFullDatabaseWithProfileAndTriageAsync(
            int numberOfTriage,
            Guid primaryProfileAccount, 
            Guid secondaryProfileAccount, 
            ApplicationDbContext dbContext,
            bool includePools = false,
            bool includePendings = false)
        {
            //smart contracts
            var smartContracts = EntityCreator.CreateSmartContract(2);
            dbContext.SmartContracts.AddRange(smartContracts);

            //profile group
            var profileGroupOne = new ProfileGroup(null, null, null, "test unit", smartContracts.ElementAt(0), 0);
            dbContext.ProfileGroups.Add(profileGroupOne);
            var profileGroupTwo = new ProfileGroup(null, null, null, "test unit", smartContracts.ElementAt(1), 1);
            dbContext.ProfileGroups.Add(profileGroupTwo);
            await dbContext.SaveChangesAsync();

            //profile group
            var accountOne = new Account("ws://test", "https://watchertest", "TestAccount", "0x12345");
            dbContext.Accounts.Add(accountOne);
            var accountTwo = new Account("ws://test2", "https://watchertest2", "TestAccount", "0x54321");
            dbContext.Accounts.Add(accountTwo);
            await dbContext.SaveChangesAsync();

            //account profile group
            var accountProfileGroupOne = new AccountProfileGroup("primaryName", primaryProfileAccount, profileGroupOne.Id, 0);
            dbContext.AccountProfileGroup.Add(accountProfileGroupOne);
            var accountProfileGroupTwo = new AccountProfileGroup("secondaryName", secondaryProfileAccount, profileGroupTwo.Id, 1);
            dbContext.AccountProfileGroup.Add(accountProfileGroupTwo);
            await dbContext.SaveChangesAsync();

            //triage
            if (numberOfTriage > 0)
            {
                var triages = CreateTransactionTriage(
                    numberOfTriage,
                    profileGroups: new List<Guid> { profileGroupOne.Id, profileGroupTwo.Id });
                dbContext.TransactionTriages.AddRange(triages);
                await dbContext.SaveChangesAsync();

            if (includePools)
            {
                var pool = CreateTransactionPool(triages);
                dbContext.TransactionPools.AddRange(pool);
                await dbContext.SaveChangesAsync();
            }
            if (includePendings)
            {
                var pending = CreateTransactionPending(triages);
                dbContext.TransactionPendings.AddRange(pending);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
