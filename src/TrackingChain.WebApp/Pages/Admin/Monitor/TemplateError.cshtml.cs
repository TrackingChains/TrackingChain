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
            public string Template { get; set; } = default!;
            public string? AddressMails { get; set; } = default!;
        }

        public TemplateErrorModel(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
            TemplateData = new TemplateModel { Template = "", AddressMails = ""};
        }

        [BindProperty]
        public TemplateModel TemplateData { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            TemplateData.Template = (await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionErrorTemplate)).Value ?? "";
            TemplateData.AddressMails = (await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionErrorMail)).Value ?? "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Salva il modello nel database
                var template = await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionErrorTemplate);
                template.Value = TemplateData.Template;
                var mails = await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionErrorMail);
                mails.Value = TemplateData.AddressMails;

                applicationDbContext.ReportSettings.Update(template);
                applicationDbContext.ReportSettings.Update(mails);
                applicationDbContext.SaveChanges();

                return RedirectToPage("Index");
            }

            ErrorMessage = "Some error in validation data";
            return Page();
        }
    }
}
