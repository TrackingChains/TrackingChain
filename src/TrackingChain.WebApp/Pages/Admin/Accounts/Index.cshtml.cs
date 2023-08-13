﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public IList<Account> Account { get;set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only

        public async Task OnGetAsync()
        {
            if (dbContext.Accounts != null)
            {
                Account = await dbContext.Accounts.ToListAsync();
            }
        }
    }
}
