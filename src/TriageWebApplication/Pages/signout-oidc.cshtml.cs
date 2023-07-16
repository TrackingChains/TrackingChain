using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrackingChain.TriageWebApplication.Pages
{
    public class signout_oidcModel : PageModel
    {
        // Methods.
        public IActionResult OnGet() =>
            SignOut(CommonConsts.UserAuthenticationPolicyScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
    }
}
