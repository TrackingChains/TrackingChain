using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public CreateModel(ApplicationDbContext context)
        {
            dbContext = context;
            SmartContractBinding = new SmartContractBinding();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SmartContractBinding SmartContractBinding { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || 
                dbContext.SmartContracts == null || 
                SmartContractBinding == null)
                return Page();

            var smartContract = new SmartContract(
                SmartContractBinding.Address,
                SmartContractBinding.ChainNumberId,
                SmartContractBinding.ChainType,
                SmartContractBinding.Currency,
                SmartContractBinding.Name,
                ContractExtraInfo.FromJson(SmartContractBinding.ExtraInfo));
            dbContext.SmartContracts.Add(smartContract);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
