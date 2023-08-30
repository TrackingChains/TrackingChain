namespace TrackingChain.Common.Enums
{
    public enum TransactionErrorReason
    {
        TransactionNotFound,
        GetTrasactionReceiptExpection,
        TransactionFinalizedInError,
        UnableToSendTransactionOnChain,
        UnableToWatchTransactionOnChain
    }
}
