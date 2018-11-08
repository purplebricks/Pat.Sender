using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Pat.Sender
{
    public interface IMessageSender
    {
        Task SendMessages(IEnumerable<Message> messages);
        Task<long> ScheduleMessage(Message message, DateTimeOffset scheduleDateTimeOffSet);
        Task CancelScheduledMessage(long sequenceNumber);
    }

}