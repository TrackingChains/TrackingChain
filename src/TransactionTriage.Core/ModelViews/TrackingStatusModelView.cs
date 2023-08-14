using System;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.Domain.Enums;

namespace TrackingChain.TransactionTriageCore.ModelViews
{
    public class TrackingStatusModelView
    {
        protected TrackingStatusModelView(
            Guid trackingIdentify,
            DateTime triageDate,
            DateTime registryDate,
            DateTime pendingDate,
            DateTime poolDate,
            RegistryStatus status,
            TransactionStep transactionStep)
        {
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
            RegistryDate = registryDate;
            PendingDate = pendingDate;
            PoolDate = poolDate;
            Status = status;
            TransactionStep = transactionStep;
        }

        // Properties.
        public DateTime RegistryDate { get; private set; }
        public RegistryStatus Status { get; private set; }
        public Guid TrackingId { get; private set; }
        public TransactionStep TransactionStep { get; private set; }
        public DateTime TriageDate { get; private set; }
        public DateTime PendingDate { get; private set; }
        public DateTime PoolDate { get; private set; }

        // Methods.
        public static TrackingStatusModelView FromEntity(TransactionRegistry transactionRegistry)
        {
            ArgumentNullException.ThrowIfNull(transactionRegistry);

            return new TrackingStatusModelView(
                transactionRegistry.TrackingId,
                transactionRegistry.TriageDate,
                transactionRegistry.RegistryDate,
                transactionRegistry.PendingDate,
                transactionRegistry.PoolDate,
                transactionRegistry.Status,
                transactionRegistry.TransactionStep);
        }
    }
}
