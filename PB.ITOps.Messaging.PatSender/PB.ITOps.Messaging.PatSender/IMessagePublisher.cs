using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessagePublisher
    {
        Task PublishEvent<TEvent>(TEvent message, IDictionary<string, string> additionalProperties = null) where TEvent : class;
        Task PublishEvents<TEvent>(IEnumerable<TEvent> messages, IDictionary<string, string> additionalProperties = null) where TEvent : class;
        Task ScheduleEvent<TEvent>(TEvent message, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null) where TEvent : class;
        Task ScheduleEvents<TEvent>(IEnumerable<TEvent> messages, DateTime scheduledEnqueueTimeUtc, IDictionary<string, string> additionalProperties = null) where TEvent : class;
    }
}
