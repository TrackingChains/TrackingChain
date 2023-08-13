using System;
using TrackingChain.Common.Enums;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class TransactionPool : TransactionBase
    {
        // Constructors.
        public TransactionPool(
            string code,
            string dataValue,
            Guid trackingIdentify,
            DateTime triageDate,
            long smartContractId,
            string smartContractAddress,
            string smartContractExtraInfo,
            Guid profileGroupId,
            int chainNumberId,
            ChainType chainType)
            : base(code, dataValue, chainNumberId, chainType, smartContractId, smartContractAddress, smartContractExtraInfo, profileGroupId)
        {
            ReceivedDate = DateTime.UtcNow;
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
            GeneratingFrom = ReceivedDate;
        }
        protected TransactionPool() { }

        // Properties.
        public Guid TrackingId { get; private set; }
        public bool Completed { get; private set; }
        public DateTime GeneratingFrom { get; private set; }
        public bool Locked { get; private set; }
        public Guid? LockedBy { get; private set; }
        public DateTime? LockedDated { get; private set; }
        public byte Priority { get; private set; }
        public DateTime TriageDate { get; private set; }
        public int ErrorTimes { get; private set; }


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

        public void Unlock(int secondsDelayGeneratingFrom = 6)
        {
            Locked = false;
            LockedBy = null;
            GeneratingFrom = DateTime.UtcNow.AddSeconds(secondsDelayGeneratingFrom);
        }

        public void UnlockFromError(int secondsDelayGeneratingFrom = 6)
        {
            ErrorTimes++;
            Locked = false;
            LockedBy = null;
            GeneratingFrom = DateTime.UtcNow.AddSeconds(secondsDelayGeneratingFrom * ErrorTimes);
        }
    }
}
