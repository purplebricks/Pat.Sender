using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessageSender
    {
        Task SendMessages(IEnumerable<Message> messages);
    }
}