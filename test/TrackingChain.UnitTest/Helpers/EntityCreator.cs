using System;
using System.Collections.Generic;
using System.Linq;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.UnitTest.Helpers
{
    internal static class EntityCreator
    {
        private static Random random = new Random();

        public static IEnumerable<TransactionTriage> CreateTransactionTriage(
            int size,
            List<Guid>? profileGroups = null)
        {
            var transactionTriages = new List<TransactionTriage>();
            if (size == 0)
                return transactionTriages;

            for (int i = 1; i <= size; i++)
            {
                var code = $"Code{i}";
                var dataValue = $"dataValue{i}";
                var profileGroup = profileGroups != null ? profileGroups[(i - 1) % profileGroups.Count] : Guid.NewGuid();
                var smartContractId = i;
                var smartContractAddress = $"0x{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}";
                var smartContractExtraInfo = $"{{testfield:{i}}}";
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

        public static IEnumerable<TransactionPending> CreateTransactionPending(IEnumerable<TransactionTriage> transactionTriages)
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
                    item.ChainType));
#pragma warning restore CA5394 // No need secure number for test

            return transactionPendings;
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
                        BasicWeight = 1,
                        ByteWeight = 2,
                        InsertTrackSelectorValue = "aaa",
                        SupportedClient = i % 2 == 0 ? SupportedClient.Shibuya : SupportedClient.Shibuya
                    }));
            }

            return smartContracts;
        }

    }
}
