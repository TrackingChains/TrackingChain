using Microsoft.AspNetCore.Mvc;
using System;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class TrackingViewBinding
    {
        // Properties.
        [BindProperty]
        public Guid TrackingId { get; set; } = default!;

        [BindProperty]
        public string? Code { get; set; } = default!;

        [BindProperty]
        public long SmartContractId { get; set; } = default!;
    }
}
