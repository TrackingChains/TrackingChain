using Microsoft.AspNetCore.Mvc;
using System;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class TrackingViewBinding
    {
        [BindProperty]
        public Guid TrackingId { get; set; } = default!;
    }
}
