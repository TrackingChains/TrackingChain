using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public DeleteModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [BindProperty]
        public bool AlreadyUsedProfileGroup { get; set; } = default!;

        [BindProperty]
        public ProfileGroupBinding ProfileGroupBinding { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || 
                dbContext.ProfileGroups == null)
                return NotFound();

            var profilegroup = await dbContext.ProfileGroups.FirstOrDefaultAsync(m => m.Id == id);

            AlreadyUsedProfileGroup = await dbContext.AccountProfileGroup.AnyAsync(m => m.ProfileGroupId == id);

            if (profilegroup == null)
                return NotFound();
            else
                ProfileGroupBinding = new ProfileGroupBinding(profilegroup);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || 
                dbContext.ProfileGroups == null)
                return NotFound();

            var profilegroup = await dbContext.ProfileGroups.FindAsync(id);

            if (profilegroup != null)
            {
                dbContext.ProfileGroups.Remove(profilegroup);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
