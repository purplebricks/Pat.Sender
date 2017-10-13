using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender
{
    public interface IMessageSender
    {
        Task SendMessages(IEnumerable<BrokeredMessage> messages);
    }
}