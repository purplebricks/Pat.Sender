using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.Extensions
{
    public static class BrokeredMessageExtensions
    {
        public static BrokeredMessage PopulateCorrelationId(this BrokeredMessage message, string correlationId)
        {
            if (!string.IsNullOrEmpty(correlationId))
            {
                message.Properties["PBCorrelationId"] = correlationId;
            }

            return message;
        }
    }
}