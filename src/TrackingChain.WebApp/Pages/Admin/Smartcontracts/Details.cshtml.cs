﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public DetailsModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

      public SmartContract SmartContract { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || dbContext.SmartContracts == null)
            {
                return NotFound();
            }

            var smartcontract = await dbContext.SmartContracts.FirstOrDefaultAsync(m => m.Id == id);
            if (smartcontract == null)
            {
                return NotFound();
            }
            else 
            {
                SmartContract = smartcontract;
            }
            return Page();
        }
    }
}
