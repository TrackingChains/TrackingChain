using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
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
            return Page();
        }

        [BindProperty]
        public SmartContract SmartContract { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || dbContext.SmartContracts == null || SmartContract == null)
            {
                return Page();
            }

            dbContext.SmartContracts.Add(SmartContract);
            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
