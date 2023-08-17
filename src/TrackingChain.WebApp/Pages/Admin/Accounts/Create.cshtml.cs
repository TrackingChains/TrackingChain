using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public CreateModel(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AccountBinding AccountBinding { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || 
               dbContext.Accounts == null || 
               AccountBinding == null)
                return Page();

            var account = new Account(
                AccountBinding.ChainWriterAddress, 
                AccountBinding.ChainWatcherAddress, 
                AccountBinding.Name, 
                AccountBinding.PrivateKey);
            dbContext.Accounts.Add(account);

            await dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
