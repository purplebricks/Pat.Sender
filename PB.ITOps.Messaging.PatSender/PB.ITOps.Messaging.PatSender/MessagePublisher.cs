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
        private readonly MessageProperties _defaultMessageProperties;

        public MessagePublisher(IMessageSender messageSender, IMessageGenerator messageGenerator, MessageProperties defaultMessageProperties)
        {
            if (messageSender == null)
            {
                throw new ArgumentNullException(nameof(messageSender));
            }

            if (messageGenerator == null)
            {
                throw new ArgumentNullException(nameof(messageGenerator));
            }

            if (defaultMessageProperties == null)
            {
                throw new ArgumentNullException(nameof(defaultMessageProperties));
            }

            _messageSender = messageSender;
            _messageGenerator = messageGenerator;
            _defaultMessageProperties = defaultMessageProperties;
        }

        /// <summary>
        /// Publishes a single event, sending it directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        /// <param name="eventSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/></param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task PublishEvent(object @event, MessageProperties eventSpecificProperties = null)
        {
            var brokeredMessage = GenerateMessage(@event, eventSpecificProperties, null);
            await _messageSender.SendMessages(new[] { brokeredMessage });
        }

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <param name="events">The set of events to publish.</param>
        /// <param name="eventSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/>. Apply to all events in this set.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task PublishEvents(IEnumerable<object> events, MessageProperties eventSpecificProperties = null)
        {
            var brokeredMessages = GenerateMessages(events, eventSpecificProperties);
            await _messageSender.SendMessages(brokeredMessages);
        }

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <param name="events">The set of events to publish along with specific properties for each event.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task PublishEventsWithProperties(IEnumerable<EventWithProperties> events)
        {
            var brokeredMessages = events.Select(
                messageWithProperties => GenerateMessage(
                    messageWithProperties.Event,
                    messageWithProperties.Properties,
                    null));

            await _messageSender.SendMessages(brokeredMessages);
        }

        /// <summary>
        /// Sends a single command to the specified subscriber.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="subscriber">The name of the subscriber to send this command to.</param>
        /// <param name="commandSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/></param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task SendCommand(object command, string subscriber, MessageProperties commandSpecificProperties = null)
        {
            var brokeredMessage = GenerateMessage(command, commandSpecificProperties, subscriber);
            await _messageSender.SendMessages(new[] { brokeredMessage });
        }

        /// <summary>
        /// Sends a collection of commands to the specified subscriber.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <param name="commands">A set of commands to send to the same subscriber.</param>
        /// <param name="subscriber">The name of the subscriber to send these commands to.</param>
        /// <param name="commandSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/>.  Apply to all commands in the set.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task SendCommands(IEnumerable<object> commands, string subscriber, MessageProperties commandSpecificProperties = null)
        {
            var brokeredMessages = GenerateMessages(commands, commandSpecificProperties, subscriber);
            await _messageSender.SendMessages(brokeredMessages);
        }

        /// <summary>
        /// Schedules a single event to be published after a delay, sending it directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// Schedules the message to be enqueued for delivery at the specified time (UTC).
        /// </summary>
        /// <param name="event">The event to trigger at a time in the future.</param>
        /// <param name="scheduledEnqueueTimeUtc">The UTC time when the event will be published.</param>
        /// <param name="eventSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/>.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task ScheduleEvent(object @event, DateTime scheduledEnqueueTimeUtc, MessageProperties eventSpecificProperties = null)
        {
            var brokeredMessage = GenerateMessage(@event, scheduledEnqueueTimeUtc, eventSpecificProperties);
            await _messageSender.SendMessages(new[] { brokeredMessage });
        }

        /// <summary>
        /// Schedules a collection of events to be published after a delay, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// Schedules each message to be enqueued for delivery at the specified time (UTC).
        /// </summary>
        /// <param name="events">The set of events to trigger at a time in the future.</param>
        /// <param name="scheduledEnqueueTimeUtc">The UTC time when the events will be published.</param>
        /// <param name="eventSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/>. Apply to all events in the set.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        public async Task ScheduleEvents(IEnumerable<object> events, DateTime scheduledEnqueueTimeUtc, MessageProperties eventSpecificProperties = null)
        {
            var brokeredMessages = GenerateMessages(events, scheduledEnqueueTimeUtc, eventSpecificProperties);
            await _messageSender.SendMessages(brokeredMessages);
        }

        private IEnumerable<BrokeredMessage> GenerateMessages(IEnumerable<object> messages, MessageProperties messageSpecificProperties = null, string subscriber = null)
            => messages.Select(message => GenerateMessage(message, messageSpecificProperties, subscriber));

        private IEnumerable<BrokeredMessage> GenerateMessages(IEnumerable<object> messages, DateTime scheduledEnqueueTimeUtc, MessageProperties messageSpecificProperties = null, string subscriber = null)
            => messages.Select(message => GenerateMessage(message, scheduledEnqueueTimeUtc, messageSpecificProperties, subscriber));

        private BrokeredMessage GenerateMessage(object message, DateTime scheduledEnqueueTimeUtc, MessageProperties messageSpecificProperties = null, string subscriber = null)
        {
            var brokeredMessage = GenerateMessage(message, messageSpecificProperties, subscriber);
            brokeredMessage.ScheduledEnqueueTimeUtc = scheduledEnqueueTimeUtc;

            return brokeredMessage;
        }

        private BrokeredMessage GenerateMessage(object message, MessageProperties messageSpecificProperties, string subscriber)
        {
            var brokeredMessage = _messageGenerator.GenerateBrokeredMessage(message);

            var messageType = message.GetType();
            brokeredMessage.MessageId = Guid.NewGuid().ToString();
            brokeredMessage.ContentType = messageType.SimpleQualifiedName();
            brokeredMessage.Properties["MessageType"] = messageType.FullName;

            brokeredMessage.PopulateCorrelationId(
                messageSpecificProperties?.CorrelationIdProvider.CorrelationId
                ?? _defaultMessageProperties.CorrelationIdProvider.CorrelationId);

            brokeredMessage.AddProperties(_defaultMessageProperties.CustomProperties);
            brokeredMessage.AddProperties(messageSpecificProperties?.CustomProperties);

            if (!string.IsNullOrEmpty(subscriber))
            {
                brokeredMessage.Properties["SpecificSubscriber"] = subscriber;
            }

            return brokeredMessage;
        }
    }
}
