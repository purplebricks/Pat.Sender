namespace PB.ITOps.Messaging.PatSender.Correlation
{
    public interface ICorrelationIdProvider
    {
        string CorrelationId { get; }
    }
}
