using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrackingChain.TransactionTriageCore.ModelViews;
using TrackingChain.TransactionTriageCore.UseCases;

namespace TrackingChain.TriageWebApplication.Pages
{
    public class MonitorModel : PageModel
    {
        // Fields.
        private readonly IAnalyticUseCase analyticUseCase;
        private readonly ILogger<MonitorModel> logger;

        // Constructors.
        public MonitorModel(
            IAnalyticUseCase analyticUseCase,
            ILogger<MonitorModel> logger)
        {
            this.analyticUseCase = analyticUseCase;
            this.logger = logger;
            TrackingStatus = new TrackingStatusStatisticModelView();
        }

        // Properties.
        public TrackingStatusStatisticModelView TrackingStatus { get; set; }

        // GET
        public async Task OnGetAsync()
        {
            TrackingStatus = await analyticUseCase.GetTrackingStatusStatisticAsync();
        }
    }
}
