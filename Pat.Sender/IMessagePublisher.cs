using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pat.Sender
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a single event, sending it directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        /// <param name="eventSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/></param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        Task PublishEvent(object @event, MessageProperties eventSpecificProperties = null);

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <param name="events">The set of events to publish.</param>
        /// <param name="eventSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/>. Apply to all events in this set.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        Task PublishEvents(IEnumerable<object> events, MessageProperties eventSpecificProperties = null);

        /// <summary>
        /// Publishes a collection of events, sending them directly to the service bus topic.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <param name="events">The set of events to publish.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        Task PublishEventsWithProperties(IEnumerable<EventWithProperties> events);

        /// <summary>
        /// Sends a single command to the specified subscriber.
        /// Sets the contentType and messageType based on the concrete event type
        /// Sets the correlation id on the message if specified.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="subscriber">The name of the subscriber to send this command to.</param>
        /// <param name="commandSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/></param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        Task SendCommand(object command, string subscriber, MessageProperties commandSpecificProperties = null);

        /// <summary>
        /// Sends a collection of commands to the specified subscriber.
        /// Sets the contentType and messageType based on the concrete event types
        /// Sets the correlation id on each message if specified.
        /// </summary>
        /// <param name="commands">A set of commands to send to the same subscriber.</param>
        /// <param name="subscriber">The name of the subscriber to send these commands to.</param>
        /// <param name="commandSpecificProperties">Properties that override the defaults set on the <see cref="IMessagePublisher"/>.  Apply to all commands in the set.</param>
        /// <returns>A <see cref="Task"/> that should be awaited to track exceptions arising, or to track completion.</returns>
        Task SendCommands(IEnumerable<object> commands, string subscriber, MessageProperties commandSpecificProperties = null);

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
        Task ScheduleEvent(object @event, DateTime scheduledEnqueueTimeUtc, MessageProperties eventSpecificProperties = null);

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
        Task ScheduleEvents(IEnumerable<object> events, DateTime scheduledEnqueueTimeUtc, MessageProperties eventSpecificProperties = null);
    }
}
