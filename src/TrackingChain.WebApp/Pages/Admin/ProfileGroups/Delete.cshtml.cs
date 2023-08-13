﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.ProfileGroups
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public DeleteModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [BindProperty]
      public ProfileGroup ProfileGroup { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.ProfileGroups == null)
            {
                return NotFound();
            }

            var profilegroup = await dbContext.ProfileGroups.FirstOrDefaultAsync(m => m.Id == id);

            if (profilegroup == null)
            {
                return NotFound();
            }
            else 
            {
                ProfileGroup = profilegroup;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || dbContext.ProfileGroups == null)
            {
                return NotFound();
            }
            var profilegroup = await dbContext.ProfileGroups.FindAsync(id);

            if (profilegroup != null)
            {
                ProfileGroup = profilegroup;
                dbContext.ProfileGroups.Remove(ProfileGroup);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}