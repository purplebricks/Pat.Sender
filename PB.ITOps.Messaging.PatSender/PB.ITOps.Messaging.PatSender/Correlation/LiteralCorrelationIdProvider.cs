namespace PB.ITOps.Messaging.PatSender.Correlation
{
    public class LiteralCorrelationIdProvider : ICorrelationIdProvider
    {
        public LiteralCorrelationIdProvider(string correlationId)
            => CorrelationId = correlationId;

        public string CorrelationId { get; }
    }
}
