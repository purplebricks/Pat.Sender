using log4net;

namespace PB.ITOps.Messaging.PatSender.Correlation
{
    public class LogicalThreadContextCorrelationIdProvider : ICorrelationIdProvider
    {
        public const string Key = "CorrelationId";

        public string CorrelationId => LogicalThreadContext.Properties[Key]?.ToString();
    }
}
