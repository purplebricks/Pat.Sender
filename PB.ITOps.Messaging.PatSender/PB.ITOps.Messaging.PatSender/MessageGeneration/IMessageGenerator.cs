using Microsoft.Azure.ServiceBus;

namespace PB.ITOps.Messaging.PatSender.MessageGeneration
{
    public interface IMessageGenerator
    {
        Message GenerateBrokeredMessage(object message);
    }
}
