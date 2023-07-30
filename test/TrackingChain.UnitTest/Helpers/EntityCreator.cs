using System;
using System.Collections.Generic;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.Core.Helpers
{
    internal static class EntityCreator
    {
        public static IEnumerable<TransactionTriage> CreateTransactionTriage(int size)
        {
            var transactionTriages = new List<TransactionTriage>();
            if (size == 0)
                return transactionTriages;

            for (int i = 1; i <= size; i++)
            {
                var code = $"Code{i}";
                var dataValue = $"dataValue{i}";
                var profileGroup = Guid.NewGuid();
                var smartContractId = i;
                var smartContractAddress = $"0x{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}{i}";
                var smartContractExtraInfo = $"{{testfield:{i}}}";
                var smartContractChainNumber = i * 100;
                var smartContractChainType = i % 2 == 0? ChainType.Substrate : ChainType.EVM;
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

        public static IEnumerable<TransactionRegistry> CreateTransactionRegistry(IEnumerable<TransactionTriage> transactionTriages)
        {
            var transactionRegistries = new List<TransactionRegistry>();

            foreach (var item in transactionTriages)
                transactionRegistries.Add( new TransactionRegistry(
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
    }
}
