using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages
{
    //[Authorize]
    public class TrackingModel : PageModel
    {
        // Fields.
        private readonly ILogger<TrackingModel> logger;
        private readonly ITrackingEntryUseCase trackingEntryUseCase;

        // Properties.
        public string? Result { get; set; }
        [BindProperty]
        public int SelectedChain { get; set; }
        public SelectList ChainOptions { get; set; }

        // Constractor.
        public TrackingModel(
            ILogger<TrackingModel> logger,
            ITrackingEntryUseCase trackingEntryUseCase)
        {
            this.logger = logger;
            this.trackingEntryUseCase = trackingEntryUseCase;
            this.ChainOptions = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Selected = false, Text = "Shibuya - (Astar)", Value = "0"},
                                        new SelectListItem { Selected = false, Text = "Moonbase - (Moonbeam)", Value = "1"},
                                    }, "Value", "Text", 1); // TODO configurable
        }

        // GET
        public void OnGet()
        {
            SelectedChain = 0;
        }

        // POST
        public async Task OnPostSubmit(TrackProductBinding trackProductBinding)
        {
            ArgumentNullException.ThrowIfNull(trackProductBinding);
            try
            {
                var result = await trackingEntryUseCase.AddTransactionAsync(
                    "GenericWabAPI",
                    trackProductBinding.ProductCode, 
                    trackProductBinding.ProductDataJson,
                    trackProductBinding.SelectedChain == 0 ? "Shibuya" : "Moonbase");
                Result = $"Status: Ok, Tx Hash: {result}";
            }
#pragma warning disable CA1031 // 
            catch (Exception ex)
            {
                Result = $"Tx Error: {ex.Message}";
            }
#pragma warning restore CA1031 // 
        }
    }
}
