using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            ViewData["ProfileGroupId"] = new SelectList(_context.ProfileGroups, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public AccountProfileGroup AccountProfileGroup { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.AccountProfileGroup == null || AccountProfileGroup == null)
            {
                return Page();
            }

            _context.AccountProfileGroup.Add(AccountProfileGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
