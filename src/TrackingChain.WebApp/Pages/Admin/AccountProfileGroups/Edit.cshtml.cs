using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.AccountProfileGroups
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public EditModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public AccountProfileGroupBinding AccountProfileGroupBinding { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? accountId, Guid? profileGroupId)
        {
            if (accountId == null ||
                profileGroupId == null ||
                dbContext.AccountProfileGroup == null)
                return NotFound();

            var accountProfileGroup = await dbContext.AccountProfileGroup
                .Include(i => i.Account)
                .Include(i => i.ProfileGroup)
                .FirstOrDefaultAsync(m => m.AccountId == accountId &&
                                          m.ProfileGroupId == profileGroupId);
            if (accountProfileGroup == null)
                return NotFound();

            AccountProfileGroupBinding = new AccountProfileGroupBinding(accountProfileGroup);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var accountProfileGroup = await dbContext.AccountProfileGroup
                .FirstOrDefaultAsync(m => m.AccountId == AccountProfileGroupBinding.AccountId &&
                                          m.ProfileGroupId == AccountProfileGroupBinding.ProfileGroupId);
            if (accountProfileGroup == null)
                return NotFound();

            accountProfileGroup.Update(
                AccountProfileGroupBinding.Name,
                AccountProfileGroupBinding.Priority);
            dbContext.Update(accountProfileGroup);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
