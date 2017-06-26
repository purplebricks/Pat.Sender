using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender
{
    public class NullMessageSender: IMessageSender
    {
        public Task SendMessages(IEnumerable<BrokeredMessage> messages)
        {
            return Task.CompletedTask;
        }
    }
}
