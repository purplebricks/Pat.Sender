using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Pat.Sender
{
    public interface IMessageSender
    {
        Task SendMessages(IEnumerable<Message> messages);
    }
}