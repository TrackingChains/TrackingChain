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
    public class RegistryDetailsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public RegistryDetailsModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

      public TransactionRegistry TransactionRegistry { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.TransactionRegistries == null)
            {
                return NotFound();
            }

            var transactionregistry = await dbContext.TransactionRegistries.FirstOrDefaultAsync(m => m.TrackingId == id);
            if (transactionregistry == null)
            {
                return NotFound();
            }
            else 
            {
                TransactionRegistry = transactionregistry;
            }
            return Page();
        }
    }
}
