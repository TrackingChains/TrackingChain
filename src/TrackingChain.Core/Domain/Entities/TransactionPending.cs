using System;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Core.Domain.Enums;

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
            ChainType chainType,
            DateTime? forceWatchingFrom = null)
            : base(code, data, chainNumberId, chainType, smartContractId, smartContractAddress, smartContractExtraInfo, profileGroupId)
        {
            ReceivedDate = DateTime.UtcNow;
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
            TxHash = txHash;
            PoolDate = poolDate;
            Status = PendingStatus.WaitingForWorker;

            var extraInfo = ContractExtraInfo.FromJson(smartContractExtraInfo);
            if (forceWatchingFrom.HasValue)
                WatchingFrom = forceWatchingFrom.Value;
            else
                WatchingFrom = extraInfo is null || extraInfo.WaitingSecondsForWatcherTx == 0 ?
                    DateTime.UtcNow.AddSeconds(90) :
                    DateTime.UtcNow.AddSeconds(extraInfo.WaitingSecondsForWatcherTx);
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected TransactionPending() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid TrackingId { get; private set; }
        public bool Completed { get; private set; }
        public int ErrorTimes { get; private set; }
        public bool IsInProgress { get; private set; }
        public bool Locked { get; private set; }
        public Guid? LockedBy { get; private set; }
        public DateTime? LockedDated { get; private set; }
        public PendingStatus Status { get; private set; }
        public DateTime TriageDate { get; private set; }
        public string TxHash { get; private set; }
        public byte Priority { get; private set; }
        public DateTime PoolDate { get; private set; }
        public DateTime WatchingFrom { get; private set; }

        // Methods.
        public void Reprocessable()
        {
            ErrorTimes = 0;
            Status = PendingStatus.WaitingForWorker;
            Unlock();
        }

        public void SetCompleted()
        {
            if (Completed)
            {
                var ex = new InvalidOperationException($"{nameof(TransactionPool)} already completed.");
                ex.Data.Add("TrackingId", TrackingId);
                throw ex;
            }

            Completed = true;
            Status = PendingStatus.Done;
        }

        public void SetStatusError()
        {
            Status = PendingStatus.Error;
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
            Status = PendingStatus.InProgress;
        }

        public void SetStatusDone()
        {
            Status = PendingStatus.Done;
        }

        public void Unlock(int secondsDelayWatchingFrom = 6)
        {
            Locked = false;
            LockedBy = null;
            Status = PendingStatus.WaitingForWorker;
            WatchingFrom = DateTime.UtcNow.AddSeconds(secondsDelayWatchingFrom);
        }

        public void UnlockFromError(int secondsDelayWatchingFrom = 6)
        {
            ErrorTimes++;
            Locked = false;
            LockedBy = null;
            Status = PendingStatus.WaitingForWorker;
            WatchingFrom = DateTime.UtcNow.AddSeconds(secondsDelayWatchingFrom * ErrorTimes);
        }
    }
}
