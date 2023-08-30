using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Trackings
{
    [Authorize]
    public class PendingDetailsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public PendingDetailsModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

      public TransactionPending TransactionPending { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.TransactionPendings == null)
            {
                return NotFound();
            }

            var transactionpending = await dbContext.TransactionPendings.FirstOrDefaultAsync(m => m.TrackingId == id);
            if (transactionpending == null)
            {
                return NotFound();
            }
            else 
            {
                TransactionPending = transactionpending;
            }
            return Page();
        }
    }
}
