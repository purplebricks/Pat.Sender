using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace PB.ITOps.Messaging.PatSender.MessageGeneration
{
    public class MessageGenerator: IMessageGenerator
    {
        public BrokeredMessage GenerateBrokeredMessage(object message)
        {
            return new BrokeredMessage(JsonConvert.SerializeObject(message));
        }
    }
}
