using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TrackingChain.TriageWebApplication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

        // Properties.
        public string? Result { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            this.logger = logger;
        }

        // GET
        public IActionResult OnGet()
        {
            return RedirectToPage("Monitor");
        }
    }
}