using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pat.Sender.Extensions
{
    internal static class MessageExtensions
    {
        public static Message AddProperties(this Message message, IDictionary<string, string> additionalProperties)
        {
            if (additionalProperties == null)
            {
                return message;
            }

            foreach (var additionalProperty in additionalProperties)
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

        public static string GetCorrelationId(this Message message)
        {
            return message.UserProperties["PBCorrelationId"]?.ToString();
        }

        /// <summary>
        /// Calculate the estimated message size in bytes.
        /// Based on: https://weblogs.asp.net/sfeldman/asb-batching-brokered-messages
        /// </summary>
        /// <param name="message">The message whose size to calculate.</param>
        /// <returns>The message size in bytes.</returns>
        public static long GetEstimatedMessageSize(this Message message)
        {
            const int assumeSize = 256;
            var standardPropertiesSize = GetStringSizeInBytes(message.MessageId) +
                                         assumeSize +   // ContentType
                                         assumeSize +   // CorrelationId
                                         4 +            // DeliveryCount
                                         8 +            // EnqueuedSequenceNumber
                                         8 +            // EnqueuedTimeUtc
                                         8 +            // ExpiresAtUtc
                                         1 +            // ForcePersistence
                                         1 +            // IsBodyConsumed
                                         assumeSize +   // Label
                                         8 +            // LockedUntilUtc 
                                         16 +           // LockToken 
                                         assumeSize +   // PartitionKey
                                         8 +            // ScheduledEnqueueTimeUtc
                                         8 +            // SequenceNumber
                                         assumeSize +   // SessionId
                                         4 +            // State
                                         8 +            // TimeToLive
                                         assumeSize +   // To
                                         assumeSize;    // ViaPartitionKey;
            var headers = message.UserProperties.Sum(property => GetStringSizeInBytes(property.Key) + GetStringSizeInBytes(property.Value.ToString()));
            var bodySize = message.Body.Length;
            var total = standardPropertiesSize + headers + bodySize;
            const int messageSizePaddingPercentage = 5;
            const double padWithPercentage = (double)(100 + messageSizePaddingPercentage) / 100;
            var estimatedSize = (long)(total * padWithPercentage);
            return estimatedSize;
        }

        private static int GetStringSizeInBytes(string value) => value != null ? Encoding.UTF8.GetByteCount(value) : 0;
    }
}