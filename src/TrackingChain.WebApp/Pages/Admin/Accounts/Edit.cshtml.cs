using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Accounts
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public EditModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [BindProperty]
        public AccountBinding AccountBinding { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.Accounts == null)
                return NotFound();

            var account =  await dbContext.Accounts.FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
                return NotFound();

            AccountBinding = new AccountBinding(account);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var account = await dbContext.Accounts.FirstOrDefaultAsync(m => m.Id == AccountBinding.Id);
            if (account == null)
                return NotFound();

            account.Update(
                AccountBinding.ChainWriterAddress,
                AccountBinding.ChainWatcherAddress,
                AccountBinding.Name,
                AccountBinding.PrivateKey);
            dbContext.Update(account);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
