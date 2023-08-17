using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages.Admin.AccountProfileGroups
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name");
            ViewData["ProfileGroupId"] = new SelectList(_context.ProfileGroups, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public AccountProfileGroupBinding AccountProfileGroupBinding { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || 
                _context.AccountProfileGroup == null || 
                AccountProfileGroupBinding == null)
                return Page();

            var accountProfileGroup = new AccountProfileGroup(
                AccountProfileGroupBinding.Name,
                AccountProfileGroupBinding.AccountId, 
                AccountProfileGroupBinding.ProfileGroupId, 
                AccountProfileGroupBinding.Priority);
            _context.AccountProfileGroup.Add(accountProfileGroup);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
