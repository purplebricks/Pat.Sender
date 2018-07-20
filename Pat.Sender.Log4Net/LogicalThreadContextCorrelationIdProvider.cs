using log4net;
using Pat.Sender.Correlation;

namespace Pat.Sender.Log4Net
{
    public class LogicalThreadContextCorrelationIdProvider : ICorrelationIdProvider
    {
        public const string Key = "CorrelationId";

        public string CorrelationId => LogicalThreadContext.Properties[Key]?.ToString();
    }
}
