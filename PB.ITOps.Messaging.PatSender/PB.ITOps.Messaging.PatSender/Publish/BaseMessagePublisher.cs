using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using PB.ITOps.Messaging.PatSender.Extentions;
using PB.ITOps.Messaging.PatSender.Send;

namespace PB.ITOps.Messaging.PatSender.Publish
{
    public abstract class BaseMessagePublisher : IMessagePublisher
    {
        protected readonly IClientBusQueue ClientBusQueue;

        protected BaseMessagePublisher(IClientBusQueue clientBusQueue)
        {
            ClientBusQueue = clientBusQueue;
        }

        private BrokeredMessage GenerateMessage(object message, string hostId = null, string subscriberName = null)
        {
            var brokeredMessage = new BrokeredMessage(JsonConvert.SerializeObject(message))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = message.GetType().SimpleQualifiedName()
            };

            brokeredMessage = DecorateMessage(brokeredMessage);

            if (!string.IsNullOrWhiteSpace(hostId))
            {
                brokeredMessage.Properties["ReplyToHostId"] = hostId;
            }

            brokeredMessage.PopulateCorrelationId(ClientBusQueue.CorrelationId);

            if (!string.IsNullOrWhiteSpace(subscriberName))
            {
                brokeredMessage.WithSpecificSubscriber(subscriberName);
            }

            return brokeredMessage;
        }

        protected virtual BrokeredMessage DecorateMessage(BrokeredMessage brokeredMessage)
        {
            var typeName = Type.GetType(brokeredMessage.ContentType);
            brokeredMessage.Properties["MessageType"] = typeName.FullName;
            return brokeredMessage;
        }

        protected abstract Task PostQueuedAction();

        private void EnqueueToLocalQueue<T>(T message, string hostId = null, string subscriberName = null) where T : class
        {
            var brokeredMessage = GenerateMessage(message, hostId, subscriberName);

            ClientBusQueue.AddMessage(brokeredMessage);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            EnqueueToLocalQueue(message);
            PostQueuedAction();
        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages) where TMessage : class
        {
            foreach (var message in messages)
            {
                EnqueueToLocalQueue(message);
            }
            PostQueuedAction();
        }
    }
}