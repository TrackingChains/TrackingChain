using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingChain.Substrate.Generic.Client.Helpers
{
    /*
     * Always group similar log delegates by type, always use incremental event ids.
     * Last event id is: 1
     */
    public static class LoggerExtensions
    {
        // Fields.
        //*** DEBUG LOGS ***
        //*** WARNING LOGS ***
        //*** ERROR LOGS ***
        private static readonly Action<ILogger, Exception> _submitExtrinsicError =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1, nameof(SubmitExtrinsicError)),
                "Error in SubmitExtrinsic");

        // Methods.
        public static void SubmitExtrinsicError(this ILogger logger, Exception exception) =>
            _submitExtrinsicError(logger, exception);
    }
}
