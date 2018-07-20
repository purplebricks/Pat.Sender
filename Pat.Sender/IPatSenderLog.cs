using System;

namespace Pat.Sender
{
    // Remove this when net451 support is dropped, using Microsoft.Extensions.Logging.Abstractions types instead.
    // Also then remove the package Pat.Sender.NetCoreLog and *consider* removing Pat.Sender.Log4Net
    public interface IPatSenderLog
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogWarning(string message, Exception exception);
        void LogCritical(string message, params object[] arguments);
    }
}
