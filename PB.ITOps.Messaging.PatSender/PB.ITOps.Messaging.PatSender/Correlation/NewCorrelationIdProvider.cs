using System;

namespace PB.ITOps.Messaging.PatSender.Correlation
{
    public class NewCorrelationIdProvider : ICorrelationIdProvider
    {
        public string CorrelationId => Guid.NewGuid().ToString();
    }
}
