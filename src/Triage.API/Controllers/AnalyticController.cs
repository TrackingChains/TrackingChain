using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TrackingChain.TransactionTriageCore.UseCases;

namespace TrackingChain.TriageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticController : ControllerBase
    {
        // Fields.
        private readonly ILogger<AnalyticController> logger;
        private readonly IAnalyticUseCase analyticUseCase;

        // Constructors.
        public AnalyticController(
            IAnalyticUseCase analyticUseCase,
            ILogger<AnalyticController> logger)
        {
            this.analyticUseCase = analyticUseCase;
            this.logger = logger;
        }

        // Get.

        /// <summary>
        /// Get tracking.
        /// </summary>
        /// <response code="200">Tracking</response>
        [HttpGet("Tracking/{trackingId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingAsync(Guid trackingId)
        {
            var tracking = await analyticUseCase.GetTrackingAsync(trackingId);
            if (tracking is null)
                return NotFound("Guid not found");

            return Ok(tracking);
        }

        /// <summary>
        /// Get failed tracking.
        /// </summary>
        /// <response code="200">Failed</response>
        [HttpGet("Tracking/Failed/{size}/{page}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingFailedsAsync(
            int size,
            int page)
        {
            int maxPageSize = 100;
            if (size == 0)
                size = 50;
            if (page == 0)
                page = 1;
            if (size > 100)
                throw new ArgumentOutOfRangeException($"Max size {maxPageSize}", nameof(maxPageSize));

            var pool = await analyticUseCase.GetTrackingFailedsAsync(size, page);
            return Ok(pool);
        }

        /// <summary>
        /// Get tracking status.
        /// </summary>
        /// <response code="200">Tracking status</response>
        [HttpGet("Tracking/Status/{trackingId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingStatusAsync(Guid trackingId)
        {
            var trackingtatus = await analyticUseCase.GetTrackingStatusAsync(trackingId);
            if (trackingtatus is null)
                return NotFound("Guid not found");

            return Ok(trackingtatus);
        }

        /// <summary>
        /// Get successfully tracking.
        /// </summary>
        /// <response code="200">Successfully</response>
        [HttpGet("Tracking/Successfully/{size}/{page}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingSuccessfullyAsync(
            int size,
            int page)
        {
            int maxPageSize = 100;
            if (size == 0)
                size = 50;
            if (page == 0)
                page = 1;
            if (size > 100)
                throw new ArgumentOutOfRangeException($"Max size {maxPageSize}", nameof(maxPageSize));

            var pool = await analyticUseCase.GetTrackingSuccessfullyAsync(size, page);
            return Ok(pool);
        }

        /// <summary>
        /// Get pools tracking.
        /// </summary>
        /// <response code="200">Pools</response>
        [HttpGet("Tracking/Pools/{size}/{page}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingPoolsAsync(
            int size,
            int page)
        {
            int maxPageSize = 100;
            if (size == 0)
                size = 50;
            if (page == 0)
                page = 1;
            if (size > 100)
                throw new ArgumentOutOfRangeException($"Max size {maxPageSize}", nameof(maxPageSize));

            var pool = await analyticUseCase.GetTrackingPoolsAsync(size, page);
            return Ok(pool);
        }

        /// <summary>
        /// Get pendings tracking.
        /// </summary>
        /// <response code="200">Pendings</response>
        [HttpGet("Tracking/Pendings/{size}/{page}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingPendingsAsync(
            int size, 
            int page)
        {
            int maxPageSize = 100;
            if (size == 0)
                size = 50;
            if (page == 0)
                page = 1;
            if (size > 100)
                throw new ArgumentOutOfRangeException($"Max size {maxPageSize}", nameof(maxPageSize));

            var pendings = await analyticUseCase.GetTrackingPendingsAsync(size, page);
            return Ok(pendings);
        }

        /// <summary>
        /// Get triages tracking.
        /// </summary>
        /// <response code="200">Triages</response>
        [HttpGet("Tracking/Triages/{size}/{page}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrackingTriagesAsync(
            int size,
            int page)
        {
            int maxPageSize = 100;
            if (size == 0)
                size = 50;
            if (page == 0)
                page = 1;
            if (size > 100)
                throw new ArgumentOutOfRangeException($"Max size {maxPageSize}", nameof(maxPageSize));

            var triages = await analyticUseCase.GetTrackingTriagesAsync(size, page);
            return Ok(triages);
        }
    }
}
