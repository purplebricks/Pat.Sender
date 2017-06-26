using System.Collections.Generic;
using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessagePublisher
    {
        Task Publish<TMessage>(TMessage message) where TMessage : class;

        Task Publish<TMessage>(IEnumerable<TMessage> messages) where TMessage : class;
    }
}
