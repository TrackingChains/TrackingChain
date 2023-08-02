using System;
using TrackingChain.Common.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionPending : TransactionBase
    {
        // Constructors.
        public TransactionPending(
            string txHash,
            string code,
            string data,
            DateTime poolDate,
            Guid trackingIdentify,
            DateTime triageDate,
            Guid profileGroupId,
            long smartContractId,
            string smartContractAddress,
            string smartContractExtraInfo,
            int chainNumberId,
            ChainType chainType)
            : base(code, data, chainNumberId, chainType, smartContractId, smartContractAddress, smartContractExtraInfo, profileGroupId)
        {
            ReceivedDate = DateTime.UtcNow;
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
            TxHash = txHash;
            PoolDate = poolDate;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected TransactionPending() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid TrackingId { get; private set; }
        public bool Completed { get; private set; }
        public bool IsInProgress { get; private set; }
        public bool Locked { get; private set; }
        public Guid? LockedBy { get; private set; }
        public DateTime? LockedDated { get; private set; }
        public DateTime TriageDate { get; private set; }
        public string TxHash { get; private set; }
        public byte Priority { get; private set; }
        public DateTime PoolDate { get; private set; }

        // Methods.
        public void SetCompleted()
        {
            if (Completed)
            {
                var ex = new InvalidOperationException($"{nameof(TransactionPool)} already completed.");
                ex.Data.Add("TrackingId", TrackingId);
                throw ex;
            }

            Completed = true;
        }

        public void SetLoked(Guid accountId)
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

        public void Unlock()
        {
            Locked= false;
        }
    }
}
