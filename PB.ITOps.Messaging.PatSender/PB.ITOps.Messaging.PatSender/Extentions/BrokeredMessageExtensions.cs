using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.Extentions
{
    public static class BrokeredMessageExtensions
    {
        public static BrokeredMessage WithSpecificSubscriber(this BrokeredMessage message, string subscriberName)
        {
            if (!string.IsNullOrWhiteSpace(subscriberName))
            {
                message.Properties["SpecificSubscriber"] = subscriberName;
            }
            return message;
        }

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