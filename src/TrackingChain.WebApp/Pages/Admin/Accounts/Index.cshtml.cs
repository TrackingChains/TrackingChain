using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public IndexModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public PaginatedList<Account> Accounts { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 1;
        public int TotalItems { get; private set; }

        public async Task OnGetAsync(int pageIndex = 1)
        {
            var query = dbContext.Accounts;

            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            Accounts = await PaginatedList<Account>.CreateAsync(query, PageIndex, PageSize);
        }
    }
}
