using Microsoft.AspNetCore.Mvc;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class TrackProductBinding
    {
        [BindProperty]
        public string ProductCode { get; set; } = default!;

        [BindProperty]
        public string ProductDataJson { get; set; } = default!;

        [BindProperty]
        public int SelectedChain { get; set; }
    }
}
