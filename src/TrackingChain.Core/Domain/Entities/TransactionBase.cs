using System;
using TrackingChain.TrackingChainCore.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Guid profileGroupId)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentException.ThrowIfNullOrEmpty(data);
            ArgumentException.ThrowIfNullOrEmpty(smartContractAddress);

            if (smartContractId <= 0)
                throw new ArgumentException($"{nameof(smartContractId)} must be great than 0");
            if (chainNumberId <= 0)
                throw new ArgumentException($"{nameof(chainNumberId)} must be great than 0");
            if (profileGroupId == Guid.Empty)
                throw new ArgumentException($"{nameof(profileGroupId)} is empty");

            Code = code; 
            Data = data;
            ReceivedDate = DateTime.UtcNow;
            ChainNumberId = chainNumberId;
            ChainType = chainType;
            SmartContractId = smartContractId;
            SmartContractAddress = smartContractAddress;
            ProfileGroupId = profileGroupId;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected TransactionBase() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public string Code { get; protected set; }
        public string Data { get; protected set; }
        public DateTime ReceivedDate { get; protected set; }
        public int ChainNumberId { get; protected set; }
        public ChainType ChainType { get; protected set; }
        public long SmartContractId { get; protected set; }
        public string SmartContractAddress { get; protected set; }
        public Guid ProfileGroupId { get; private set; }
    }
}
