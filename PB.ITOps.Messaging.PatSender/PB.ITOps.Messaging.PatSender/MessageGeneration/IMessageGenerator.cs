using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.MessageGeneration
{
    public interface IMessageGenerator
    {
        BrokeredMessage GenerateBrokeredMessage(object message);
    }
}
