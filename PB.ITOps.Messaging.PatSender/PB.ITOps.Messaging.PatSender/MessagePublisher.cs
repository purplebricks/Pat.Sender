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
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public async Task PublishEvent<TEvent>(TEvent message, IDictionary<string, string> additionalProperties = null) where TEvent : class
        {
            var brokeredMessage = GenerateMessage(message, additionalProperties, null);
            await _messageSender.SendMessages(new[] {brokeredMessage});
        }

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="messages"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public async Task PublishEvents<TEvent>(IEnumerable<TEvent> messages, IDictionary<string, string> additionalProperties = null) where TEvent : class
        {
            var brokeredMessages = GenerateMessages(messages, additionalProperties);
            await _messageSender.SendMessages(brokeredMessages);
        }

        /// <summary>
        /// Sends a single command to the specified subscriber.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="command"></param>
        /// <param name="subscriber"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public async Task SendCommand<TEvent>(TEvent command, string subscriber, IDictionary<string, string> additionalProperties = null) where TEvent : class
        {
            var brokeredMessage = GenerateMessage(command, additionalProperties, subscriber);
            await _messageSender.SendMessages(new[] { brokeredMessage });

        }

        /// <summary>
        /// Sends a collection of commands to the specified subscriber.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="commands"></param>
        /// <param name="subscriber"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public async Task SendCommands<TEvent>(IEnumerable<TEvent> commands, string subscriber, IDictionary<string, string> additionalProperties = null) where TEvent : class
        {
            var brokeredMessages = GenerateMessages(commands, additionalProperties, subscriber);
            await _messageSender.SendMessages(brokeredMessages);
        }


        /// <summary>
        /// Schedules a single event to be published after a delay, sending it directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// Schedules the message to be enqueued for delivery at the specified time (UTC).
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="message"></param>
        /// <param name="scheduledEnqueueTimeUtc"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public async Task ScheduleEvent<TEvent>(TEvent message, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null) where TEvent : class
        {
            var brokeredMessage = GenerateMessage(message, scheduledEnqueueTimeUtc, additionalProperties);
            await _messageSender.SendMessages(new[] { brokeredMessage });
        }

        /// <summary>
        /// Schedules a collection of events to be published after a delay, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// Schedules each message to be enqueued for delivery at the specified time (UTC).
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="messages"></param>
        /// <param name="scheduledEnqueueTimeUtc"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        public async Task ScheduleEvents<TEvent>(IEnumerable<TEvent> messages, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null) where TEvent : class
        {
            var brokeredMessages = GenerateMessages(messages, scheduledEnqueueTimeUtc, additionalProperties);
            await _messageSender.SendMessages(brokeredMessages);
        }

        private IEnumerable<BrokeredMessage> GenerateMessages(IEnumerable<object> messages, IDictionary<string, string> additionalProperties = null, string subscriber = null)
            => messages.Select(message => GenerateMessage(message, additionalProperties, subscriber));

        private IEnumerable<BrokeredMessage> GenerateMessages(IEnumerable<object> messages, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null, string subscriber = null)
            => messages.Select(message => GenerateMessage(message, scheduledEnqueueTimeUtc, additionalProperties, subscriber));

        private BrokeredMessage GenerateMessage(object message, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null, string subscriber = null)
        {
            var brokeredMessage = GenerateMessage(message, additionalProperties, subscriber);
            brokeredMessage.ScheduledEnqueueTimeUtc = scheduledEnqueueTimeUtc;

            return brokeredMessage;
        }

        private BrokeredMessage GenerateMessage(object message, IDictionary<string, string> additionalProperties, string subscriber)
        {
            var brokeredMessage = _messageGenerator.GenerateBrokeredMessage(message);

            var messageType = message.GetType();
            brokeredMessage.MessageId = Guid.NewGuid().ToString();
            brokeredMessage.ContentType = messageType.SimpleQualifiedName();
            brokeredMessage.Properties["MessageType"] = messageType.FullName;
            brokeredMessage.PopulateCorrelationId(_correlationId);
            brokeredMessage.AddProperties(_customProperties);
            brokeredMessage.AddProperties(additionalProperties);
            if (!string.IsNullOrEmpty(subscriber))
            {
                brokeredMessage.Properties["SpecificSubscriber"] = subscriber;
            }

            return brokeredMessage;
        }
    }
}
