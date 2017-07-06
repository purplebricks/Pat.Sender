using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using PB.ITOps.Messaging.PatSender.Extensions;

namespace PB.ITOps.Messaging.PatSender
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageSender _messageSender;
        private readonly string _correlationId;

        public MessagePublisher(IMessageSender messageSender, string correlationId = null)
        {
            _messageSender = messageSender;
            _correlationId = correlationId;
        }

        public async Task Publish<TMessage>(TMessage message) where TMessage : class
        {
            var brokeredMessage = GenerateMessage(message);
            await _messageSender.SendMessages(new[] {brokeredMessage});
        }

        public async Task Publish<TMessage>(IEnumerable<TMessage> messages) where TMessage : class
        {
            var brokeredMessages = GenerateMessages(messages);
            await _messageSender.SendMessages(brokeredMessages);
        }

        private IEnumerable<BrokeredMessage> GenerateMessages(IEnumerable<object> messages)
            => messages.Select(GenerateMessage);

        private BrokeredMessage GenerateMessage(object message)
        {
            var messageType = message.GetType();

            var brokeredMessage = new BrokeredMessage(JsonConvert.SerializeObject(message))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = messageType.SimpleQualifiedName()
            };

            brokeredMessage.Properties["MessageType"] = messageType.FullName;

            brokeredMessage.PopulateCorrelationId(_correlationId);

            return brokeredMessage;
        }
    }
}
