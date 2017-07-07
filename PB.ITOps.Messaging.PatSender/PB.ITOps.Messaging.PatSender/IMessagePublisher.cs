using System.Collections.Generic;
using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessagePublisher
    {
        Task PublishEvent<TEvent>(TEvent message) where TEvent : class;

        Task PublishEvents<TEvent>(IEnumerable<TEvent> messages) where TEvent : class;
    }
}
