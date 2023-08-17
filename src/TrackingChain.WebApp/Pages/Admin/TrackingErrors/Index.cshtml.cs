using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.TrackingErrors
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public IndexModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public PaginatedList<TransactionRegistry> TransactionRegistries { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 1;
        public int TotalItems { get; private set; }

        public async Task OnGetAsync(int pageIndex = 1)
        {
            var query = dbContext.TransactionRegistries
                .Where(tr => tr.Status == RegistryStatus.Error);

            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            TransactionRegistries = await PaginatedList<TransactionRegistry>.CreateAsync(query, PageIndex, PageSize);
        }
    }
}
