using Microsoft.ServiceBus.Messaging;
using PB.ITOps.Messaging.PatSender.Extensions;
using PB.ITOps.Messaging.PatSender.MessageGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageSender _messageSender;
        private readonly IMessageGenerator _messageGenerator;
        private readonly string _correlationId;
        private readonly IDictionary<string, string> _customProperties;

        public MessagePublisher(IMessageSender messageSender, IMessageGenerator messageGenerator, string correlationId, IDictionary<string, string> customProperties = null)
        {
            _messageSender = messageSender;
            _messageGenerator = messageGenerator;
            _customProperties = customProperties;
            _correlationId = correlationId;
        }

        /// <summary>
        /// Publishes a single event, sending it directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishEvent<TEvent>(TEvent message) where TEvent : class
        {
            var brokeredMessage = GenerateMessage(message);
            await _messageSender.SendMessages(new[] {brokeredMessage});
        }

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="messages"></param>
        /// <returns></returns>
        public async Task PublishEvents<TEvent>(IEnumerable<TEvent> messages) where TEvent : class
        {
            var brokeredMessages = GenerateMessages(messages);
            await _messageSender.SendMessages(brokeredMessages);
        }

        private IEnumerable<BrokeredMessage> GenerateMessages(IEnumerable<object> messages)
            => messages.Select(GenerateMessage);

        private BrokeredMessage GenerateMessage(object message)
        {
            var brokeredMessage = _messageGenerator.GenerateBrokeredMessage(message);

            var messageType = message.GetType();
            brokeredMessage.MessageId = Guid.NewGuid().ToString();
            brokeredMessage.ContentType = messageType.SimpleQualifiedName();
            brokeredMessage.Properties["MessageType"] = messageType.FullName;
            brokeredMessage.PopulateCorrelationId(_correlationId);

            if (_customProperties != null)
            {
                foreach (var customerProperty in _customProperties)
                {
                    brokeredMessage.Properties[customerProperty.Key] = customerProperty.Value;
                }
            }

            return brokeredMessage;
        }
    }
}
