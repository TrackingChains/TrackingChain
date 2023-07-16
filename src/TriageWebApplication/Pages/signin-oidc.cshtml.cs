using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrackingChain.TriageWebApplication.Pages
{
    [Authorize]
    public class signin_oidcModel : PageModel
    {
        // Methods.
        public IActionResult OnGetAsync(string? returnUrl = null) =>
             Redirect(returnUrl ?? Url.Content("~/"));
    }
}
