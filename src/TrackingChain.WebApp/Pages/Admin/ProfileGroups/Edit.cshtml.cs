using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public EditModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [BindProperty]
        public ProfileGroupBinding ProfileGroupBinding { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.ProfileGroups == null)
            {
                return NotFound();
            }

            var profilegroup = await dbContext.ProfileGroups.FirstOrDefaultAsync(m => m.Id == id);
            if (profilegroup == null)
            {
                return NotFound();
            }

            ProfileGroupBinding = new ProfileGroupBinding(profilegroup);
            ViewData["SmartContractId"] = new SelectList(dbContext.SmartContracts, "Id", "Name");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var profileGroup = await dbContext.ProfileGroups.FirstOrDefaultAsync(m => m.Id == ProfileGroupBinding.Id);
            if (profileGroup == null)
                return NotFound();

            var smartContract = await dbContext.SmartContracts.FirstOrDefaultAsync(m => m.Id == ProfileGroupBinding.SmartContractId);
            if (smartContract == null)
                return NotFound();

            profileGroup.Update(
               ProfileGroupBinding.AggregationCode,
               ProfileGroupBinding.Authority,
               ProfileGroupBinding.Category,
               ProfileGroupBinding.Name,
               ProfileGroupBinding.SmartContractId,
               ProfileGroupBinding.Priority);
            dbContext.Update(profileGroup);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
