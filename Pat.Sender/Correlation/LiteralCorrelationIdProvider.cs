namespace Pat.Sender.Correlation
{
    public class LiteralCorrelationIdProvider : ICorrelationIdProvider
    {
        public LiteralCorrelationIdProvider(string correlationId)
            => CorrelationId = correlationId;

        public string CorrelationId { get; }
    }
}
