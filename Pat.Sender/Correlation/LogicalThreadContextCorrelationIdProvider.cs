using log4net;

namespace Pat.Sender.Correlation
{
    public class LogicalThreadContextCorrelationIdProvider : ICorrelationIdProvider
    {
        public const string Key = "CorrelationId";

        public string CorrelationId => LogicalThreadContext.Properties[Key]?.ToString();
    }
}
