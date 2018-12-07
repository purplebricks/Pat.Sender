using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Pat.Sender.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Pat.Sender.Legacy
{
    public class LegacyMessagePublisher : ILegacyMessagePublisher
    {
        private readonly IMessageSender _messageSender;
        private readonly string _correlationId;

        public LegacyMessagePublisher(IMessageSender messageSender, string correlationId = null)
        {
            _messageSender = messageSender;
            _correlationId = correlationId;
        }

        public async Task PublishLegacyMessage<TLegacyMessage>(TLegacyMessage legacyMessage, string legacyMessageType, string legacyContentType) where TLegacyMessage : class
        {
            var message = GenerateMessage(legacyMessage, legacyMessageType, legacyContentType);
            await _messageSender.SendMessages(new[] { message }).ConfigureAwait(false);
        }

        private Message GenerateMessage(object payload, string legacyMessageType, string legacyContentType)
        {
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = legacyContentType
            };

            message.UserProperties["MessageType"] = legacyMessageType;
            message.PopulateCorrelationId(_correlationId);

            return message;
        }
    }
}
