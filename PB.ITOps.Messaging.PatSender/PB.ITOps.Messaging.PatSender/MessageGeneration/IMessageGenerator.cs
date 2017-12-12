using System;
using Microsoft.Azure.ServiceBus;

namespace PB.ITOps.Messaging.PatSender.MessageGeneration
{
    public interface IMessageGenerator
    {
        [Obsolete("Use GenerateMessage Instead")]
        Message GenerateBrokeredMessage(object message);
        Message GenerateMessage(object message);
    }
}
