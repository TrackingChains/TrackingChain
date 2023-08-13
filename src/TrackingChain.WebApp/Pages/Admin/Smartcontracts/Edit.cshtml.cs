using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public EditModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [BindProperty]
        public SmartContract SmartContract { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || dbContext.SmartContracts == null)
            {
                return NotFound();
            }

            var smartcontract =  await dbContext.SmartContracts.FirstOrDefaultAsync(m => m.Id == id);
            if (smartcontract == null)
            {
                return NotFound();
            }
            SmartContract = smartcontract;
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

            dbContext.Attach(SmartContract).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SmartContractExists(SmartContract.Id))
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

        private bool SmartContractExists(long id)
        {
          return (dbContext.SmartContracts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
