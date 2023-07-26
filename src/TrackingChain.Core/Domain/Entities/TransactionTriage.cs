using System;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionTriage : TransactionBase
    {
        // Constructors.
        public TransactionTriage(
            string code, 
            string data,
            Guid profileGroupId,
            long smartContractId,
            string smartContractAddress,
            int chainNumberId,
            ChainType chainType)
            : base (code, data, chainNumberId, chainType, smartContractId, smartContractAddress, profileGroupId)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentException.ThrowIfNullOrEmpty(data);
            ArgumentException.ThrowIfNullOrEmpty(smartContractAddress);

            if (smartContractId <= 0)
                throw new ArgumentException($"{nameof(smartContractId)} must be great than 0");

            Data = data;
            ReceivedDate = DateTime.UtcNow;
            TrackingIdentify = Guid.NewGuid();
        }
        protected TransactionTriage() { }

        // Properties.
        public long Id { get; private set; }
        public bool Completed { get; private set; }
        public bool IsInPool { get; private set; }
        public Guid TrackingIdentify { get; private set; }

        // Methods.
        public void SetCompleted()
        {
            Completed = true;
        }

        public void SetInPool()
        {
            IsInPool = true;
        }
    }
}
