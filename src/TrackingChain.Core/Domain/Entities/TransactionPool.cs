using System;
using TrackingChain.Common.Enums;
using TrackingChain.Core.Domain.Enums;

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
        public int ErrorTimes { get; private set; }
        public DateTime GeneratingFrom { get; private set; }
        public TransactionErrorReason? LastUnlockedError { get; private set; }
        public bool Locked { get; private set; }
        public Guid? LockedBy { get; private set; }
        public DateTime? LockedDated { get; private set; }
        public byte Priority { get; private set; }
        public PoolStatus Status { get; private set; }
        public DateTime TriageDate { get; private set; }

        // Methods.
        public void Reprocessable()
        {
            ErrorTimes = 0;
            Status = PoolStatus.WaitingForWorker;
            Unlock();
        }

        public void SetCompleted()
        {
            Completed = true;
            Status = PoolStatus.Done;
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
            Status = PoolStatus.InProgress;
        }

        public void SetStatusDone()
        {
            Status = PoolStatus.Done;
        }

        public void SetStatusError()
        {
            Status = PoolStatus.Error;
        }

        public void Unlock(int secondsDelayGeneratingFrom = 6)
        {
            Locked = false;
            LockedBy = null;
            GeneratingFrom = DateTime.UtcNow.AddSeconds(secondsDelayGeneratingFrom);
            Status = PoolStatus.WaitingForWorker;
        }

        public void UnlockFromError(
            TransactionErrorReason transactionErrorReason, 
            int secondsDelayGeneratingFrom = 6)
        {
            ErrorTimes++;
            LastUnlockedError = transactionErrorReason;
            Locked = false;
            LockedBy = null;
            GeneratingFrom = DateTime.UtcNow.AddSeconds(secondsDelayGeneratingFrom * ErrorTimes);
            Status = PoolStatus.WaitingForWorker;
        }
    }
}
