using System;
using TrackingChain.Common.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    abstract public class TransactionBase
    {
        // Constructors.
        protected TransactionBase(
            string code,
            string data,
            int chainNumberId,
            ChainType chainType,
            long smartContractId,
            string smartContractAddress,
            string smartContractExtraInfo,
            Guid profileGroupId)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentException.ThrowIfNullOrEmpty(data);
            ArgumentException.ThrowIfNullOrEmpty(smartContractExtraInfo);
            ArgumentException.ThrowIfNullOrEmpty(smartContractAddress);

            if (smartContractId <= 0)
                throw new ArgumentException($"{nameof(smartContractId)} must be great than 0");
            if (chainNumberId <= 0)
                throw new ArgumentException($"{nameof(chainNumberId)} must be great than 0");
            if (profileGroupId == Guid.Empty)
                throw new ArgumentException($"{nameof(profileGroupId)} is empty");

            Code = code; 
            DataValue = data;
            ChainNumberId = chainNumberId;
            ChainType = chainType;
            SmartContractExtraInfo = smartContractExtraInfo;
            ReceivedDate = DateTime.UtcNow;
            SmartContractId = smartContractId;
            SmartContractAddress = smartContractAddress;
            ProfileGroupId = profileGroupId;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected TransactionBase() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public string Code { get; protected set; }
        public string DataValue { get; protected set; }
        public DateTime ReceivedDate { get; protected set; }
        public int ChainNumberId { get; protected set; }
        public ChainType ChainType { get; protected set; }
        public string SmartContractExtraInfo { get; private set; }
        public long SmartContractId { get; protected set; }
        public string SmartContractAddress { get; protected set; }
        public Guid ProfileGroupId { get; private set; }
    }
}
