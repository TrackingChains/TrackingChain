using Microsoft.AspNetCore.Mvc;
using System;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class TrackingViewBinding
    {
        [BindProperty]
        public Guid Id { get; set; } = default!;
    }
}
