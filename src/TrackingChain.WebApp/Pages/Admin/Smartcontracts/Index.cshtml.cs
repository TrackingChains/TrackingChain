using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public IndexModel(ApplicationDbContext context)
        {
            dbContext = context;
            NameSort = "";
            CurrentFilter = "";
            CurrentSort = "";
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public PaginatedList<SmartContract> SmartContracts { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
        //pagination
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 1;
        public int TotalItems { get; private set; }
        //sort and filter
        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(
            string sortOrder,
            string currentFilter,
            string searchString,
            int pageIndex = 1)
        {
            // Filter.
            if (searchString != null)
                pageIndex = 1;
            else
                searchString = currentFilter;
            CurrentFilter = searchString;

            var query = dbContext.SmartContracts.AsNoTracking();
            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(s => s.Name.Contains(searchString));

            // Order.
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(s => s.Name),
                _ => query.OrderBy(s => s.Name),
            };
            CurrentSort = sortOrder;

            // Pagination.
            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            SmartContracts = await PaginatedList<SmartContract>.CreateAsync(query, PageIndex, PageSize);
        }
    }
}
