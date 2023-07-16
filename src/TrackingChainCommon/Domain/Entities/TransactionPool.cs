using System;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionPool : TransactionBase
    {
        // Constructors.
        public TransactionPool(
            string code,
            string data,
            Guid trackingIdentify,
            DateTime triageDate,
            long smartContractId,
            string smartContractAddress,
            Guid profileGroupId,
            int chainNumberId,
            ChainType chainType)
            : base(code, data, chainNumberId, chainType, smartContractId, smartContractAddress, profileGroupId)
        {
            ReceivedDate = DateTime.UtcNow;
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
        }
        protected TransactionPool() { }

        // Properties.
        public Guid TrackingId { get; private set; }
        public bool Completed { get; private set; }
        public bool Locked { get; private set; }
        public Guid? LockedBy { get; private set; }
        public DateTime? LockedDated { get; private set; }
        public DateTime TriageDate { get; private set; }
        public byte Priority { get; private set; }

        // Methods.
        public void SetCompleted()
        {
            Completed = true;
        }

        public void SetLocked(Guid accountId)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentNullException(nameof(accountId));

            if (Locked)
            {
                var ex = new InvalidOperationException($"{nameof(TransactionPool)} already locked.");
                ex.Data.Add("TrackingId", TrackingId);
                throw ex;
            }

            Locked = true;
            LockedBy = accountId;
            LockedDated = DateTime.UtcNow;
        }
    }
}
