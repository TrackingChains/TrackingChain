using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public CreateModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult OnGet()
        {
        ViewData["SmartContractId"] = new SelectList(dbContext.SmartContracts, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public ProfileGroup ProfileGroup { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || dbContext.ProfileGroups == null || ProfileGroup == null)
            {
                return Page();
            }

            dbContext.ProfileGroups.Add(ProfileGroup);
            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
