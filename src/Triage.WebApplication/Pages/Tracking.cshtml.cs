using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TriageWebApplication.ModelBinding;

namespace TrackingChain.TriageWebApplication.Pages
{
    //[Authorize]
    public class TrackingModel : PageModel
    {
        // Fields.
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<TrackingModel> logger;
        private readonly ITrackingEntryUseCase trackingEntryUseCase;

        // Properties.
        public string? Result { get; set; }
        [BindProperty]
        public int SelectedChain { get; set; }
        public SelectList ChainOptions { get; set; } = default!;

        // Constractor.
        public TrackingModel(
            ApplicationDbContext dbContext,
            ILogger<TrackingModel> logger,
            ITrackingEntryUseCase trackingEntryUseCase)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.trackingEntryUseCase = trackingEntryUseCase;
        }

        // GET
        public async Task OnGet()
        {
            await PopulateComboBoxAsync();
        }

        // POST
        public async Task OnPostSubmit(TrackProductBinding trackProductBinding)
        {
            ArgumentNullException.ThrowIfNull(trackProductBinding);

            await PopulateComboBoxAsync();
            try
            {
                var result = await trackingEntryUseCase.AddTransactionAsync(
                    "GenericWabAPI",
                    trackProductBinding.ProductCode,
                    trackProductBinding.ProductDataJson,
                    ChainOptions.ElementAt(trackProductBinding.SelectedChain).Text);
                Result = $"Status: Ok, Tx Hash: {result}";
            }
#pragma warning disable CA1031 // 
            catch (Exception ex)
            {
                Result = $"Tx Error: {ex.Message}";
            }
#pragma warning restore CA1031 // 
        }

        // Helpers.
        private async Task PopulateComboBoxAsync()
        {
            var categories = await dbContext.ProfileGroups
                .Select(pg => pg.Category)
                .Distinct()
                .OrderBy(category => category)
                .ToListAsync();
            this.ChainOptions = new SelectList(
                categories
                    .Select((category, index) => new SelectListItem
                    {
                        Selected = false,
                        Text = category,
                        Value = index.ToString(CultureInfo.InvariantCulture)
                    }),
                "Value",
                "Text",
                1);
        }
    }
}
