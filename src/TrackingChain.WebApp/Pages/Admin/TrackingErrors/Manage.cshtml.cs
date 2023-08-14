using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Domain.Entities;
using System.Linq;

namespace TrackingChain.TriageWebApplication.Pages.Admin.TrackingErrors
{
    public class ManageModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public ManageModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public TransactionRegistry TransactionRegistry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || dbContext.Accounts == null)
            {
                return NotFound();
            }

            var transactionRegistry = await dbContext.TransactionRegistries.FirstOrDefaultAsync(m => m.TrackingId == id);
            if (transactionRegistry == null)
            {
                return NotFound();
            }
            TransactionRegistry = transactionRegistry;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(
            Guid trackingId, 
            string buttonRecoveryAction)
        {
            var transactionRegistry = await dbContext.TransactionRegistries.FirstOrDefaultAsync(m => m.TrackingId == trackingId);
            if (transactionRegistry is null)
                return NotFound();
            TransactionRegistry = transactionRegistry;

            if (buttonRecoveryAction == "ReTry")
                TransactionRegistry.SetWaitingToReTry();
            else if (buttonRecoveryAction == "Cancel")
                TransactionRegistry.SetToCanceled();

            dbContext.Attach(TransactionRegistry).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistryExists(TransactionRegistry.TrackingId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RegistryExists(Guid id)
        {
            return (dbContext.TransactionRegistries?.Any(e => e.TrackingId == id)).GetValueOrDefault();
        }
    }
}
