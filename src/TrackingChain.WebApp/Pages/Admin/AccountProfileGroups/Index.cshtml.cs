using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.AccountProfileGroups
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public IndexModel(ApplicationDbContext context)
        {
            dbContext = context;
            NameSort = "";
            AccountNameSort = "";
            ProfileGroupNameSort = "";
            CurrentFilter = "";
            CurrentSort = "";
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public PaginatedList<AccountProfileGroup> AccountProfileGroups { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
        //pagination
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 1;
        public int TotalItems { get; private set; }
        //sort and filter
        public string NameSort { get; set; }
        public string AccountNameSort { get; set; }
        public string ProfileGroupNameSort { get; set; }
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

            var query = dbContext.AccountProfileGroup
                .AsNoTracking();
            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(s => s.Name.Contains(searchString) ||
                                         s.ProfileGroup.Name.Contains(searchString) ||
                                         s.Account.Name.Contains(searchString));

            // Order.
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            AccountNameSort = sortOrder == "AccountName" ? "account_name_desc" : "AccountName";
            ProfileGroupNameSort = sortOrder == "ProfileGroupName" ? "profile_group_name_desc" : "ProfileGroupName";
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(s => s.Name),
                "account_name_desc" => query.OrderByDescending(s => s.Account.Name),
                "profile_group_name_desc" => query.OrderByDescending(s => s.ProfileGroup.Name),
                "AccountName" => query.OrderBy(s => s.Account.Name),
                "ProfileGroupName" => query.OrderBy(s => s.ProfileGroup.Name),
                _ => query.OrderBy(s => s.Name),
            };
            CurrentSort = sortOrder;

            // Pagination.
            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            AccountProfileGroups = await PaginatedList<AccountProfileGroup>.CreateAsync(
                query
                .Include(a => a.Account)
                .Include(a => a.ProfileGroup), 
                PageIndex, 
                PageSize);
        }
    }
}
