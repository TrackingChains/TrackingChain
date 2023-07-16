using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TriageAPI.ModelBinding;

namespace TrackingChain.TriageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackingEntryController : ControllerBase
    {
        // Fields.
        private readonly ILogger<TrackingEntryController> logger;
        private readonly ITrackingEntryUseCase trackingEntryUseCase;

        // Constructors.
        public TrackingEntryController(
            ILogger<TrackingEntryController> logger,
            ITrackingEntryUseCase trackingEntryUseCase)
        {
            this.logger = logger;
            this.trackingEntryUseCase = trackingEntryUseCase;
        }

        // Post.

        /// <summary>
        /// Get product status info.
        /// </summary>
        /// <response code="200">Product info</response>
        [HttpPost("Product")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertProductStatusAsync(InsertTransactionPoolBinding insertTransactionPoolBinding)
        {
            if (insertTransactionPoolBinding is null)
                return BadRequest();

            var trackId = await trackingEntryUseCase.AddTransactionAsync(
                insertTransactionPoolBinding.Code,
                insertTransactionPoolBinding.Data,
                insertTransactionPoolBinding.Category);

            return Ok(trackId);
        }
    }
}