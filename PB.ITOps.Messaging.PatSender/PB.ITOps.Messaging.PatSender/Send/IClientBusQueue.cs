using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.Send
{
    public interface IClientBusQueue
    {
        void AddMessage(BrokeredMessage message);
        void AddMessages(IEnumerable<BrokeredMessage> messages);
        Task SendAll();
        Task SendAll(Action<BrokeredMessage> topicMessageDecorator);
        void ClearAll();
        string CorrelationId { get; }
    }
}
