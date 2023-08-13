using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

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
        public ProfileGroup ProfileGroup { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.ProfileGroups == null)
            {
                return NotFound();
            }

            var profilegroup =  await dbContext.ProfileGroups.FirstOrDefaultAsync(m => m.Id == id);
            if (profilegroup == null)
            {
                return NotFound();
            }
            ProfileGroup = profilegroup;
           ViewData["SmartContractId"] = new SelectList(dbContext.SmartContracts, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            dbContext.Attach(ProfileGroup).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileGroupExists(ProfileGroup.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProfileGroupExists(Guid id)
        {
          return (dbContext.ProfileGroups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
