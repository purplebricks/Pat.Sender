using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace PB.ITOps.Messaging.PatSender.MessageGeneration
{
    public class MessageGenerator: IMessageGenerator
    {
        public Message GenerateBrokeredMessage(object payload)
        {
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        }

        public Message GenerateMessage(object payload)
        {
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        }
    }
}
