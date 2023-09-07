namespace TrackingChain.Common.Enums
{
    public enum TransactionErrorReason
    {
        GetTrasactionReceiptExpection = 0,
        InsertTransactionExpection = 1,
        LockedTimeOut = 2,
        UnableToSendTransactionOnChain = 3,
        UnableToWatchTransactionOnChain = 4,
        TransactionNotFound = 5,
        TransactionFinalizedInError = 6
    }
}
