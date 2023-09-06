using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Monitor
{
    public class TemplateErrorModel : PageModel
    {
        private readonly ApplicationDbContext applicationDbContext;

        public class TemplateModel
        {
            public int Id { get; set; }
            public string ContenutoHtml { get; set; } = default!;
            public string IndirizziEmail { get; set; } = default!;
            public string TitoloEmail { get; set; } = default!;
        }

        public TemplateErrorModel(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        [BindProperty]
        public TemplateModel Model { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Model.ContenutoHtml = (await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionErrorTemplate))?.Value ?? "";
            Model.TitoloEmail = (await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionErrorTitle))?.Value ?? "";
            Model.IndirizziEmail = (await applicationDbContext.ReportSettings.FirstOrDefaultAsync(rs => rs.Key == ReportSetting.TransactionErrorMail))?.Value ?? "";
        }

        public IActionResult OnPost()
        {
            /*
            if (ModelState.IsValid)
            {
                // Salva il modello nel database
                _context.Templates.Add(Model);
                _context.SaveChanges();

                TempData["Message"] = "Template salvato con successo";
                return RedirectToPage("/Index"); // Reindirizza a una pagina di conferma o all'indice
            }
            */
            return Page();
        }
    }
}
