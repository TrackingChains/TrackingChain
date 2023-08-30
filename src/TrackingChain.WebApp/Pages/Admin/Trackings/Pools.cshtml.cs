﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using Microsoft.AspNetCore.Authorization;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Trackings
{
    [Authorize]
    public class PoolsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public PoolsModel(ApplicationDbContext context)
        {
            dbContext = context;
            CodeSort = "";
            ReceivedDateSort = "";
            TriageDateSort = "";
            CurrentFilter = "";
            CurrentSort = "";
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public PaginatedList<TransactionPool> TransactionPools { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
        //pagination
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 1;
        public int TotalItems { get; private set; }
        //sort and filter
        public string CodeSort { get; set; }
        public string ReceivedDateSort { get; set; }
        public string TriageDateSort { get; set; }
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

            var query = dbContext.TransactionPools.AsNoTracking();
            if (!string.IsNullOrEmpty(searchString))
            {
                if (Guid.TryParse(searchString, out var trackId))
                    query = query.Where(s => s.TrackingId == trackId);
                else
                    query = query.Where(s => s.Code.Contains(searchString));
            }

            // Order.
            CodeSort = sortOrder == "Code" ? "code_desc" : "Code";
            ReceivedDateSort = string.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            TriageDateSort = sortOrder == "TriageDate" ? "triage_data_desc" : "TriageDate";
            query = sortOrder switch
            {
                "code_desc" => query.OrderByDescending(s => s.Code),
                "date_asc" => query.OrderBy(s => s.ReceivedDate),
                "Code" => query.OrderBy(s => s.Code),
                _ => query.OrderByDescending(s => s.ReceivedDate),
            };
            CurrentSort = sortOrder;

            // Pagination.
            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            TransactionPools = await PaginatedList<TransactionPool>.CreateAsync(query, PageIndex, PageSize);
        }
    }
}
