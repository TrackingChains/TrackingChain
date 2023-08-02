using System;
using TrackingChain.Common.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionTriage : TransactionBase
    {
        // Constructors.
        public TransactionTriage(
            string code, 
            string dataValue,
            Guid profileGroupId,
            long smartContractId,
            string smartContractAddress,
            string smartContractExtraInfo,
            int chainNumberId,
            ChainType chainType)
            : base (code, dataValue, chainNumberId, chainType, smartContractId, smartContractAddress, smartContractExtraInfo, profileGroupId)
        {
            if (smartContractId <= 0)
                throw new ArgumentException($"{nameof(smartContractId)} must be great than 0");

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
