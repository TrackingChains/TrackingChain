using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TransactionTriageCore.ModelViews;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TriageWebApplication.ModelBinding;
using TrackingChain.TriageWebApplication.ModelView;

namespace TrackingChain.TriageWebApplication.Pages
{
    //[Authorize]
    public class TrackingViewerModel : PageModel
    {
        // Fields.
        private readonly IAnalyticUseCase analyticUseCase;
        private readonly IEnumerable<IBlockchainService> blockchainServices;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<TrackingViewerModel> logger;

        // Constructors.
        public TrackingViewerModel(
            IAnalyticUseCase analyticUseCase,
            ApplicationDbContext dbContext,
            IEnumerable<IBlockchainService> blockchainServices,
            ILogger<TrackingViewerModel> logger)
        {
            this.analyticUseCase = analyticUseCase;
            this.blockchainServices = blockchainServices;
            this.dbContext = dbContext;
            this.logger = logger;
            trackingProduct = new();
        }

        // Properties.
        public string? Result { get; set; }
        private List<TrackingDataModelView> trackingProduct { get; set; }
        public IReadOnlyCollection<TrackingDataModelView> TrackingProductModelViews { get { return trackingProduct; } }

        // GET
        public void OnGet()
        {
        }

        // POST
        public async Task OnPostSubmitAsync(TrackingViewBinding trackingViewBinding)
        {
            ArgumentNullException.ThrowIfNull(trackingViewBinding);

            IEnumerable<TrackingModelView> trackingModelViews;
            if (trackingViewBinding.TrackingId == Guid.Empty)
                trackingModelViews = await analyticUseCase.GetTrackingHistoryAsync(trackingViewBinding.Code ?? "", trackingViewBinding.SmartContractId);
            else
                trackingModelViews = await analyticUseCase.GetTrackingHistoryAsync(trackingViewBinding.TrackingId);

            foreach (var itemTracked in trackingModelViews)
            {
                var item = new TrackingDataModelView();
                item.Code = itemTracked.Code;
                item.BlockNumber = itemTracked.ReceiptBlockNumber ?? "Missing Receipt";
                item.DataValue = itemTracked.DataValue;
                item.Timestamp = itemTracked.RegistryDate;
                trackingProduct.Add(item);
            }

            /*
             * TODO for Milestone 4 (missing implementation of GetTrasactionDataAsync() and integration with SubstrateGame library)
             * 
            var trackingModelView = await analyticUseCase.GetTrackingAsync(trackingViewBinding.TrackingId);
            if (trackingModelView is null)
                return;

            var blockChainService = blockchainServices.First(x => x.ProviderType == trackingModelView.ChainType);

            if (string.IsNullOrWhiteSpace(trackingModelView.SmartContractEndpoint))
            {
                var ex = new InvalidOperationException("Missing endpoint for GetTrasactionDataAsync");
                ex.Data.Add("TrackingId", trackingViewBinding.TrackingId);
                throw ex;
            }

            var trackingChainData = await blockChainService.GetTrasactionDataAsync(
                trackingModelView.Code,
                trackingModelView.SmartContractAddress,
                trackingModelView.SmartContractEndpoint,
                trackingModelView.ChainNumberId,
                ContractExtraInfo.FromJson(trackingModelView.SmartContractExtraInfo),
                CancellationToken.None);

            if (trackingChainData is null)
            {
                Result = "Not found";
                return;
            }

            Result = "Ok";
            trackingProduct.Clear();
            foreach (var itemTracked in trackingChainData.DataValues)
            {
                var item = new TrackingModelView();
                item.Code = trackingChainData.Code;
                item.BlockNumber = itemTracked.BlockNumber;
                item.DataValue = Encoding.UTF8.GetString(itemTracked.DataValue).TrimEnd('\0');
                item.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(itemTracked.Timestamp).ToLocalTime();
                trackingProduct.Add(item);
            }
            */
        }
    }
}
