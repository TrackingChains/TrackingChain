using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Smartcontracts
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public CreateModel(ApplicationDbContext context)
        {
            dbContext = context;
            ErrorMessage = "";
            SmartContractBinding = new SmartContractBinding();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SmartContractBinding SmartContractBinding { get; set; } = default!;
        public string ErrorMessage { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || 
                dbContext.SmartContracts == null || 
                SmartContractBinding == null)
                return Page();

            ContractExtraInfo contractExtraInfo;
            try
            {
                contractExtraInfo = ContractExtraInfo.FromJson(SmartContractBinding.ExtraInfo);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                ErrorMessage = "Contract ExtraInfo json not valid";
                return Page();
            }
#pragma warning restore CA1031 // Do not catch general exception types

            var smartContract = new SmartContract(
                SmartContractBinding.Address,
                SmartContractBinding.ChainNumberId,
                SmartContractBinding.ChainType,
                SmartContractBinding.Currency,
                SmartContractBinding.Name,
                contractExtraInfo);
            dbContext.SmartContracts.Add(smartContract);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
