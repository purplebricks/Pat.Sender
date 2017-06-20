using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.Send
{
    public interface IMessageSender
    {
        Task SendMessages(IList<BrokeredMessage> messages);
    }
}