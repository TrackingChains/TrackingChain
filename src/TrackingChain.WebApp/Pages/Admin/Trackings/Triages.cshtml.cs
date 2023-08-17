using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Trackings
{
    public class TriagesModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public TriagesModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public PaginatedList<TransactionTriage> TransactionTriages { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 1;
        public int TotalItems { get; private set; }

        public async Task OnGetAsync(int pageIndex = 1)
        {
            var query = dbContext.TransactionTriages;

            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            TransactionTriages = await PaginatedList<TransactionTriage>.CreateAsync(query, PageIndex, PageSize);
        }
    }
}
