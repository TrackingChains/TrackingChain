﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Trackings
{
    public class RegistriesModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public RegistriesModel(ApplicationDbContext context)
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
            var query = dbContext.TransactionRegistries;

            TotalItems = await query.CountAsync();

            PageIndex = pageIndex;
            TransactionRegistries = await PaginatedList<TransactionRegistry>.CreateAsync(query, PageIndex, PageSize);
        }
    }
}
