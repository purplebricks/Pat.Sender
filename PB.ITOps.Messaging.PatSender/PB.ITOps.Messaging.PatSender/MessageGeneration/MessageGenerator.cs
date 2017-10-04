using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace PB.ITOps.Messaging.PatSender.MessageGeneration
{
    public class MessageGenerator: IMessageGenerator
    {
        public Message GenerateBrokeredMessage(object message)
        {
            return new Message(MessageEncoding.Instance.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}
