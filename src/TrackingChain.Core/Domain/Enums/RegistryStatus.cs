namespace TrackingChain.Core.Domain.Enums
{
    public enum RegistryStatus
    {
        Aborted = 4,
        CanceledDueToError = 3,
        Error = 2,
        InProgress = 0,
        SuccessfullyCompleted = 1,
        WaitingToReTry = 5,
        WaitingToCancel = 6
    }
}
