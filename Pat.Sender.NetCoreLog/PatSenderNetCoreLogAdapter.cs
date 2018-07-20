using Microsoft.Extensions.Logging;
using System;

namespace Pat.Sender.NetCoreLog
{
    public class PatSenderNetCoreLogAdapter : IPatSenderLog
    {
        private readonly ILogger log;

        public PatSenderNetCoreLogAdapter(ILogger log)
        {
            this.log = log;
        }

        public void LogCritical(string message, params object[] arguments)
            => log.LogCritical(message, arguments);

        public void LogInformation(string message)
            => log.LogInformation(message);

        public void LogWarning(string message)
            => log.LogWarning(message);

        public void LogWarning(string message, Exception exception)
            => log.LogWarning(exception, message);
    }
}
