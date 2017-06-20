using System.Collections.Generic;

namespace PB.ITOps.Messaging.PatSender.Publish
{
    public interface IMessagePublisher
    {
        void Publish<TMessage>(TMessage message) where TMessage : class;

        void Publish<TMessage>(IEnumerable<TMessage> messages) where TMessage : class;
    }
}