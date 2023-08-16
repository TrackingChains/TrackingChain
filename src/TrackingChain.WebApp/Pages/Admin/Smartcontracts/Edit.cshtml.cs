using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public EditModel(ApplicationDbContext context)
        {
            dbContext = context;
            SmartContractBinding = new SmartContractBinding();
        }

        [BindProperty]
        public SmartContractBinding SmartContractBinding { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || dbContext.SmartContracts == null)
                return NotFound();

            var smartContract =  await dbContext.SmartContracts.FirstOrDefaultAsync(m => m.Id == id);
            if (smartContract == null)
                return NotFound();

            SmartContractBinding = new SmartContractBinding(smartContract);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var smartContract = await dbContext.SmartContracts.FirstOrDefaultAsync(m => m.Id == SmartContractBinding.Id);
            if (smartContract == null)
                return NotFound();

            smartContract.Update(
               SmartContractBinding.Address,
               SmartContractBinding.ChainNumberId,
               SmartContractBinding.ChainType,
               SmartContractBinding.Currency,
               SmartContractBinding.ExtraInfo,
               SmartContractBinding.Name);
            dbContext.Update(smartContract);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
