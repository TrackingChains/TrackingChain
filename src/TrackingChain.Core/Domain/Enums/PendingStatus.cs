﻿namespace TrackingChain.Core.Domain.Enums
{
    public enum PendingStatus
    {
        Done = 2,
        Error = 3,
        InProgress = 1,
        Excluded = 4,
        WaitingForWorker = 0,
    }
}
