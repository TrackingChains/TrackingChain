using Microsoft.AspNetCore.Mvc;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class TrackingBinding
    {
        [BindProperty]
        public string Code { get; set; } = default!;

        [BindProperty]
        public string DataValue { get; set; } = default!;

        [BindProperty]
        public int SelectedChain { get; set; }
    }
}
