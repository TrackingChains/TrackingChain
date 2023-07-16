using System.Collections.Generic;
using System;

namespace TrackingChain.TransactionWatcherWorker.Options
{
    public class CheckerOptions
    {
        public int Instance { get; set; }
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
        public List<Guid> Accounts { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists
    }
}
