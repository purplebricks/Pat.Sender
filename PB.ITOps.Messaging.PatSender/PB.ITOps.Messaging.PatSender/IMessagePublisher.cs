using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a single event, sending it directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="message"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        Task PublishEvent<TEvent>(TEvent message, IDictionary<string, string> additionalProperties = null) where TEvent : class;

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="messages"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        Task PublishEvents<TEvent>(IEnumerable<TEvent> messages, IDictionary<string, string> additionalProperties = null) where TEvent : class;

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
        Task SendCommand<TEvent>(TEvent command, string subscriber, IDictionary<string, string> additionalProperties = null) where TEvent : class;

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
        Task SendCommands<TEvent>(IEnumerable<TEvent> commands, string subscriber, IDictionary<string, string> additionalProperties = null) where TEvent : class;

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
        Task ScheduleEvent<TEvent>(TEvent message, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null) where TEvent : class;

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
        Task ScheduleEvents<TEvent>(IEnumerable<TEvent> messages, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null) where TEvent : class;
    }
}
