using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Trackings
{
    public class PendingsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public PendingsModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<TransactionPending> TransactionPending { get;set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only

        public async Task OnGetAsync()
        {
            if (dbContext.TransactionPendings != null)
            {
                TransactionPending = await dbContext.TransactionPendings.ToListAsync();
            }
        }
    }
}
