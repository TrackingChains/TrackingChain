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
    public class PoolDetailsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public PoolDetailsModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

      public TransactionPool TransactionPool { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.TransactionPools == null)
            {
                return NotFound();
            }

            var transactionpool = await dbContext.TransactionPools.FirstOrDefaultAsync(m => m.TrackingId == id);
            if (transactionpool == null)
            {
                return NotFound();
            }
            else 
            {
                TransactionPool = transactionpool;
            }
            return Page();
        }
    }
}
