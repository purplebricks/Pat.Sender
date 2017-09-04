using System.Collections.Generic;
using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessagePublisher
    {
        Task PublishEvent<TEvent>(TEvent message, IDictionary<string, string> additionalProperties = null) where TEvent : class;
        Task PublishEvents<TEvent>(IEnumerable<TEvent> messages, IDictionary<string, string> additionalProperties = null) where TEvent : class;
    }
}
