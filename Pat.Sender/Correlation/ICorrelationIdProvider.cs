namespace Pat.Sender.Correlation
{
    public interface ICorrelationIdProvider
    {
        string CorrelationId { get; }
    }
}
