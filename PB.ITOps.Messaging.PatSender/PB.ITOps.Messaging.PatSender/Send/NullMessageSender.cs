using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.Send
{
    public class NullMessageSender: IMessageSender
    {
        public Task SendMessages(IList<BrokeredMessage> messages)
        {
            return Task.CompletedTask;
        }
    }
}
