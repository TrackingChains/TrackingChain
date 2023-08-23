using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.AccountProfileGroups
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
            ViewData["AccountId"] = new SelectList(dbContext.Accounts, "Id", "Name");
            ViewData["ProfileGroupId"] = new SelectList(dbContext.ProfileGroups, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public AccountProfileGroupBinding AccountProfileGroupBinding { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || 
                dbContext.AccountProfileGroup == null || 
                AccountProfileGroupBinding == null)
                return Page();

            if (! await dbContext.Accounts.AnyAsync(a => a.Id == AccountProfileGroupBinding.AccountId))
                return NotFound();
            if (! await dbContext.ProfileGroups.AnyAsync(a => a.Id == AccountProfileGroupBinding.ProfileGroupId))
                return NotFound();

            var accountProfileGroup = new AccountProfileGroup(
                AccountProfileGroupBinding.Name,
                AccountProfileGroupBinding.AccountId, 
                AccountProfileGroupBinding.ProfileGroupId, 
                AccountProfileGroupBinding.Priority);
            dbContext.AccountProfileGroup.Add(accountProfileGroup);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
