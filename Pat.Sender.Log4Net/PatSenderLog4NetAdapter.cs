using log4net;
using System;

namespace Pat.Sender.Log4Net
{
    /// <inheritdoc />
    public class PatSenderLog4NetAdapter<T> : IPatSenderLog<T>
    {
        private readonly ILog log;

        public PatSenderLog4NetAdapter(ILog log)
        {
            this.log = log;
        }

        /// <inheritdoc />
        public void LogCritical(string message)
            => log.FatalFormat(message);

        /// <inheritdoc />
        public void LogInformation(string message)
            => log.Info(message);

        /// <inheritdoc />
        public void LogWarning(string message)
            => log.Warn(message);

        /// <inheritdoc />
        public void LogWarning(string message, Exception exception)
            => log.Warn(message, exception);
    }
}
