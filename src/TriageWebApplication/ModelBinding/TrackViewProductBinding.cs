using Microsoft.AspNetCore.Mvc;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class TrackViewProductBinding
    {
        [BindProperty]
        public string ProductCode { get; set; } = default!;
    }
}
