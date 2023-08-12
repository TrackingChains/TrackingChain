using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.TriageWebApplication.ModelView;

namespace TrackingChain.TriageWebApplication.Pages
{
    //[Authorize]
    public class TrackingViewModel : PageModel
    {
        // Fields.
        private readonly IAnalyticUseCase analyticUseCase;
        private readonly IEnumerable<IBlockchainService> blockchainServices;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<TrackingViewModel> logger;

        // Constructors.
        public TrackingViewModel(
            IAnalyticUseCase analyticUseCase,
            ApplicationDbContext dbContext,
            IEnumerable<IBlockchainService> blockchainServices,
            ILogger<TrackingViewModel> logger)
        {
            this.analyticUseCase = analyticUseCase;
            this.blockchainServices = blockchainServices;
            this.dbContext = dbContext;
            this.logger = logger;
            trackingProduct = new ();
        }

        // Properties.
        public string? Result { get; set; }
        public string? ProductCode { get; set; }
        private List<TrackingProductModelView> trackingProduct { get; set; }
        public IReadOnlyCollection<TrackingProductModelView> TrackingProductModelViews { get { return trackingProduct; } }

        // Methods.
        public async Task OnGetAsync(TrackingViewBinding trackingViewBinding)
        {
            ArgumentNullException.ThrowIfNull(trackingViewBinding);

                var trackingModelView = await analyticUseCase.GetTrackingAsync(trackingViewBinding.Id);
                if (trackingModelView is null)
                    return;

                var blockChainService = blockchainServices.First(x => x.ProviderType == trackingModelView.ChainType);
                /*
                var result = await blockChainService.GetTrasactionDataAsync(
                    trackingModelView.Code,
                    trackingModelView.SmartContractAddress,
                    trackingModelView.end);
                Result = "Ok";
                ProductCode = trackingViewBinding.Code;
                _trackingProduct = new List<TrackingProductModelView>();
                foreach (var tracked in result.ReturnValue1)
                {
                    var item = new TrackingProductModelView();
                    item.ProductCode = trackingViewBinding.Code;
                    item.BlockNumber = tracked.BlockNumber.ToString();
                    item.DataValue = tracked.DataValue;
                    item.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((long)tracked.Timestamp).ToLocalTime();
                    _trackingProduct.Add(item);
                }*/
        }
    }
}
