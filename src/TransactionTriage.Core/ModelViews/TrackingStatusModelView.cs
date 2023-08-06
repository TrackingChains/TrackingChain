using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
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
            bool? receiptSuccessful,
            TransactionStep transactionStep)
        {
            TrackingId = trackingIdentify;
            TriageDate = triageDate;
            RegistryDate = registryDate;
            PendingDate = pendingDate;
            PoolDate = poolDate;
            ReceiptSuccessful= receiptSuccessful;
            TransactionStep = transactionStep;
        }

        // Properties.
        public bool? ReceiptSuccessful { get; private set; }
        public DateTime RegistryDate { get; private set; }
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
                transactionRegistry.ReceiptSuccessful,
                transactionRegistry.TransactionStep);
        }
    }
}
