using PB.ITOps.Messaging.PatSender.Extensions;
using PB.ITOps.Messaging.PatSender.MessageGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace PB.ITOps.Messaging.PatSender
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageSender _messageSender;
        private readonly IMessageGenerator _messageGenerator;
        private readonly MessageProperties _defaultMessageProperties;

        public MessagePublisher(IMessageSender messageSender, IMessageGenerator messageGenerator, MessageProperties defaultMessageProperties)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _messageGenerator = messageGenerator ?? throw new ArgumentNullException(nameof(messageGenerator));
            _defaultMessageProperties = defaultMessageProperties ?? throw new ArgumentNullException(nameof(defaultMessageProperties));
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
            var message = GenerateMessage(@event, eventSpecificProperties, null);
            await _messageSender.SendMessages(new[] { message });
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
            var messages = GenerateMessages(events, eventSpecificProperties);
            await _messageSender.SendMessages(messages);
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
            var messages = events.Select(
                messageWithProperties => GenerateMessage(
                    messageWithProperties.Event,
                    messageWithProperties.Properties,
                    null));

            await _messageSender.SendMessages(messages);
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
            var message = GenerateMessage(command, commandSpecificProperties, subscriber);
            await _messageSender.SendMessages(new[] { message });
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
            var messages = GenerateMessages(commands, commandSpecificProperties, subscriber);
            await _messageSender.SendMessages(messages);
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
            var message = GenerateMessage(@event, scheduledEnqueueTimeUtc, eventSpecificProperties);
            await _messageSender.SendMessages(new[] { message });
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
            var messages = GenerateMessages(events, scheduledEnqueueTimeUtc, eventSpecificProperties);
            await _messageSender.SendMessages(messages);
        }

        private IEnumerable<Message> GenerateMessages(IEnumerable<object> messagePayloads, MessageProperties messageSpecificProperties = null, string subscriber = null)
            => messagePayloads.Select(payload => GenerateMessage(payload, messageSpecificProperties, subscriber));

        private IEnumerable<Message> GenerateMessages(IEnumerable<object> messagePayloads, DateTime scheduledEnqueueTimeUtc, MessageProperties messageSpecificProperties = null, string subscriber = null)
            => messagePayloads.Select(payload => GenerateMessage(payload, scheduledEnqueueTimeUtc, messageSpecificProperties, subscriber));

        private Message GenerateMessage(object payload, DateTime scheduledEnqueueTimeUtc, MessageProperties messageSpecificProperties = null, string subscriber = null)
        {
            var message = GenerateMessage(payload, messageSpecificProperties, subscriber);
            message.ScheduledEnqueueTimeUtc = scheduledEnqueueTimeUtc;

            return message;
        }

        private Message GenerateMessage(object payload, MessageProperties messageSpecificProperties, string subscriber)
        {
            var message = _messageGenerator.GenerateMessage(payload);

            var messageType = payload.GetType();
            message.MessageId = Guid.NewGuid().ToString();
            message.ContentType = messageType.SimpleQualifiedName();
            message.UserProperties["MessageType"] = messageType.FullName;

            message.PopulateCorrelationId(
                messageSpecificProperties?.CorrelationIdProvider.CorrelationId
                ?? _defaultMessageProperties.CorrelationIdProvider.CorrelationId);

            message.AddProperties(_defaultMessageProperties.CustomProperties);
            message.AddProperties(messageSpecificProperties?.CustomProperties);

            if (!string.IsNullOrEmpty(subscriber))
            {
                message.UserProperties["SpecificSubscriber"] = subscriber;
            }

            return message;
        }
    }
}
