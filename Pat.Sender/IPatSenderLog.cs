using System;

namespace Pat.Sender
{
    /// <summary>
    /// Represents a type used to perform logging.
    /// </summary>
    /// <remarks>
    /// Remove this when net451 support is dropped, using Microsoft.Extensions.Logging.Abstractions types instead.
    /// Also then remove the package Pat.Sender.NetCoreLog and *consider* removing Pat.Sender.Log4Net
    /// </remarks>
    public interface IPatSenderLog
    {
        /// <summary>
        /// Writes an informational message to the log.
        /// </summary>
        /// <param name="message">The log entry</param>
        void LogInformation(string message);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="message">The log entry</param>
        void LogWarning(string message);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="message">The log entry</param>
        /// <param name="exception">A related exception</param>
        void LogWarning(string message, Exception exception);

        /// <summary>
        /// Writes a critical message to the log.
        /// </summary>
        /// <param name="message">The log entry</param>
        void LogCritical(string message);
    }

    public interface IPatSenderLog<T> : IPatSenderLog
    {
    }
}
