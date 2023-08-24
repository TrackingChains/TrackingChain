using System;
using System.Globalization;
using System.Text;

namespace TrackingChain.Core.Extensions
{
    public static class TrackinChainExceptionExtensions
    {
        public static string GetAllExceptionMessages(this Exception ex)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            StringBuilder messages = new(ex.Message);
#pragma warning restore CA1062 // Validate arguments of public methods

            if (ex.InnerException != null)
                messages = messages.AppendLine(CultureInfo.InvariantCulture, $"Inner Exception: {GetAllExceptionMessages(ex.InnerException)}");

            return messages.ToString();
        }
    }
}
