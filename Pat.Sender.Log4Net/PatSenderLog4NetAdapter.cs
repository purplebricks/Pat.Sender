using log4net;
using System;

namespace Pat.Sender.Log4Net
{
    public class PatSenderLog4NetAdapter : IPatSenderLog
    {
        private readonly ILog log;

        public PatSenderLog4NetAdapter(ILog log)
        {
            this.log = log;
        }

        public void LogCritical(string message, params object[] arguments)
            => log.FatalFormat(message, arguments);

        public void LogInformation(string message)
            => log.Info(message);

        public void LogWarning(string message)
            => log.Warn(message);

        public void LogWarning(string message, Exception exception)
            => log.Warn(message, exception);
    }
}
