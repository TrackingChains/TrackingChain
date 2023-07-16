using Microsoft.AspNetCore.Mvc;

namespace TrackingChain.TriageAPI.ModelBinding
{
    public class InsertTransactionPoolBinding
    {
        [BindProperty]
        public string Code { get; set; } = default!;

        [BindProperty]
        public string Data { get; set; } = default!;

        [BindProperty]
        public string Category { get; set; } = default!;
    }
}
