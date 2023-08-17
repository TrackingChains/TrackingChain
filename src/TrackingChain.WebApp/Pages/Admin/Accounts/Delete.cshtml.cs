using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Accounts
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public DeleteModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [BindProperty]
        public bool AlreadyUsedAccount { get; set; } = default!;

        [BindProperty]
        public AccountBinding AccountBinding { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.Accounts == null)
                return NotFound();

            var account = await dbContext.Accounts.FirstOrDefaultAsync(m => m.Id == id);

            AlreadyUsedAccount = await dbContext.AccountProfileGroup.AnyAsync(m => m.AccountId == id);

            if (account == null)
                return NotFound();
            else
                AccountBinding = new AccountBinding(account);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || dbContext.Accounts == null)
                return NotFound();

            var account = await dbContext.Accounts.FindAsync(id);

            if (account != null)
            {
                dbContext.Accounts.Remove(account);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
