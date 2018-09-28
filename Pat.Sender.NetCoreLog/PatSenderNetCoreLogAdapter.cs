using Microsoft.Extensions.Logging;
using System;

namespace Pat.Sender.NetCoreLog
{
    /// <inheritdoc />
    public class PatSenderNetCoreLogAdapter<T> : IPatSenderLog<T>
    {
        private readonly ILogger log;

        public PatSenderNetCoreLogAdapter(ILogger<T> log)
        {
            this.log = log;
        }
        
        /// <inheritdoc />
        public void LogCritical(string message)
            => log.LogCritical(message);

        /// <inheritdoc />
        public void LogInformation(string message)
            => log.LogInformation(message);

        /// <inheritdoc />
        public void LogWarning(string message)
            => log.LogWarning(message);

        /// <inheritdoc />
        public void LogWarning(string message, Exception exception)
            => log.LogWarning(exception, message);
    }
}
