using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups
{
    [Authorize]
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
        public ProfileGroupBinding ProfileGroupBinding { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid ||
                  dbContext.ProfileGroups == null ||
                  ProfileGroupBinding == null)
                return Page();

            var smartContarct = await dbContext.SmartContracts.FirstOrDefaultAsync(sc => sc.Id == ProfileGroupBinding.SmartContractId);
            if (smartContarct == null)
                return NotFound();

            var profileGroup = new ProfileGroup(
                ProfileGroupBinding.AggregationCode,
                ProfileGroupBinding.Authority,
                ProfileGroupBinding.Category,
                ProfileGroupBinding.Name,
                smartContarct,
                ProfileGroupBinding.Priority);
            dbContext.ProfileGroups.Add(profileGroup);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
