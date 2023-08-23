using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Trackings
{
    public class TriageDetailsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public TriageDetailsModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

      public TransactionTriage TransactionTriage { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || dbContext.TransactionTriages == null)
            {
                return NotFound();
            }

            var transactiontriage = await dbContext.TransactionTriages.FirstOrDefaultAsync(m => m.Id == id);
            if (transactiontriage == null)
            {
                return NotFound();
            }
            else 
            {
                TransactionTriage = transactiontriage;
            }
            return Page();
        }
    }
}
