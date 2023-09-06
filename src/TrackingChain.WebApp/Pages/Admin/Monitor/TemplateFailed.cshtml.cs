using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

namespace TrackingChain.TriageWebApplication.Pages.Admin.Monitor
{
    public class TemplateFailedModel : PageModel
    {
        private readonly ApplicationDbContext applicationDbContext;

        public class TemplateModel
        {
            public string Template { get; set; } = default!;
            public string AddressMails { get; set; } = default!;
            public string Title { get; set; } = default!;
        }

        public TemplateFailedModel(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
            TemplateData = new TemplateModel { Template = "", AddressMails = "", Title = "" };
        }

        [BindProperty]
        public TemplateModel TemplateData { get; set; } = default!;
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            TemplateData.Template = (await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionCancelledTemplate)).Value ?? "";
            TemplateData.AddressMails = (await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionCancelledMail)).Value ?? "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Salva il modello nel database
                var template = await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionCancelledTemplate);
                template.Value = TemplateData.Template;
                var mails = await applicationDbContext.ReportSettings.FirstAsync(rs => rs.Key == ReportSetting.TransactionCancelledMail);
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
