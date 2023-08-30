using Microsoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

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
            if (id == null || 
                dbContext.Accounts == null)
                return NotFound();

            var transactionRegistry = await dbContext.TransactionRegistries.FirstOrDefaultAsync(m => m.TrackingId == id);
            if (transactionRegistry == null)
                return NotFound();

            TransactionRegistry = transactionRegistry;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            Guid trackingId, 
            string buttonRecoveryAction)
        {
            var transactionRegistry = await dbContext.TransactionRegistries.FirstOrDefaultAsync(m => m.TrackingId == trackingId);
            if (transactionRegistry is null)
                return NotFound();
            TransactionRegistry = transactionRegistry;

            if (buttonRecoveryAction == "ReTry To Process")
                TransactionRegistry.SetWaitingToReTry();
            else if (buttonRecoveryAction == "Cancel for Error")
                TransactionRegistry.SetToCanceled();
            else
            {
                var ex = new InvalidOperationException();
                ex.AddData("ButtonRecoveryAction", buttonRecoveryAction);
                ex.AddData("TrackingId", trackingId);
                throw ex;
            }


            dbContext.Update(transactionRegistry);
            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
