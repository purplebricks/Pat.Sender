using System;

namespace Pat.Sender.Correlation
{
    public class NewCorrelationIdProvider : ICorrelationIdProvider
    {
        public string CorrelationId => Guid.NewGuid().ToString();
    }
}
