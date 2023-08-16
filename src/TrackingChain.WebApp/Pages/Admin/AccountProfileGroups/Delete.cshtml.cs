using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.AccountProfileGroups
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public DeleteModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public AccountProfileGroup AccountProfileGroup { get; set; } = default!;

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

            AccountProfileGroup = accountProfileGroup;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? accountId, Guid? profileGroupId)
        {
            if (accountId == null ||
                profileGroupId == null ||
                dbContext.AccountProfileGroup == null)
                return NotFound();

            var accountprofilegroup = await dbContext.AccountProfileGroup
                .Include(i => i.Account)
                .Include(i => i.ProfileGroup)
                .FirstOrDefaultAsync(m => m.AccountId == accountId &&
                                          m.ProfileGroupId == profileGroupId);
            if (accountprofilegroup != null)
            {
                AccountProfileGroup = accountprofilegroup;
                dbContext.AccountProfileGroup.Remove(AccountProfileGroup);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
