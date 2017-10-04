using System.Linq;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;

namespace PB.ITOps.Messaging.PatSender.Extensions
{
    public static class MessageExtensions
    {
        public static Message AddProperties(this Message message, IDictionary<string, string> additionalProperties)
        {
            foreach (var additionalProperty in additionalProperties ?? new Dictionary<string, string>())
            {
                message.UserProperties[additionalProperty.Key] = additionalProperty.Value;
            }

            return message;
        }

        public static Message PopulateCorrelationId(this Message message, string correlationId)
        {
            if (!string.IsNullOrEmpty(correlationId))
            {
                message.UserProperties["PBCorrelationId"] = correlationId;
            }

            return message;
        }

        public static long GetSize(this Message message)
        {
            long estimatedSize = 61;
            int minimumFieldSize = 8;

            estimatedSize += message.ContentType.Length;
            foreach (var propertyPair in message.UserProperties.AsEnumerable())
            {
                estimatedSize += propertyPair.Key.Length + 5;
                if (propertyPair.Value != null)
                {
                    estimatedSize += Math.Max(minimumFieldSize, propertyPair.Value.ToString().Length);
                }
            }
            estimatedSize += message.Size;
            return estimatedSize;
        }
    }
}