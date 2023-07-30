using System;
using System.Collections.Generic;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.Core.Helpers
{
    internal static class EntityCreator
    {
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
