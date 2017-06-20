using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender.Send
{
    public class ClientBusQueue : IClientBusQueue
    {
        private readonly ILog _log;
        private List<BrokeredMessage> _topicMessages;
        private readonly IMessageSender _messageSender;
        private readonly PatSenderSettings _senderSettings;

        public string CorrelationId { get; }

        public ClientBusQueue(
            ILog log,
            IMessageSender messageSender,
            PatSenderSettings senderSettings,
            string correlationId = null)
        {
            _log = log;
            _messageSender = messageSender;
            _senderSettings = senderSettings;
            _topicMessages = new List<BrokeredMessage>();
            CorrelationId = correlationId;
        }

        public void AddMessage(BrokeredMessage message)
        {
            _topicMessages.Add(message);
        }

        public void AddMessages(IEnumerable<BrokeredMessage> messages)
        {
            _topicMessages.AddRange(messages);
        }

        public async Task SendAll()
        {
            Action<BrokeredMessage> emptyDecorator = x => { };
            await SendAll(emptyDecorator);
        }

        public async Task SendAll(Action<BrokeredMessage> topicMessageDecorator)
        {
            // Nasty race condition averted
            var topicMessages = Interlocked.Exchange(ref _topicMessages, new List<BrokeredMessage>());

            try
            {
                if (topicMessages.Any())
                {
                    foreach (var topicMessage in topicMessages)
                    {
                        topicMessageDecorator(topicMessage);
                    }
                    await _messageSender.SendMessages(topicMessages);
                }
            }
            catch (Exception ex)
            {
                var queueDetails = new StringBuilder();

                queueDetails.AppendLine("PostQueuedAction Error Occurred (Logging Queue Details):");
                queueDetails.AppendLine($"Main topic queue name: {_senderSettings.TopicName}; item count:{topicMessages.Count()}");
                queueDetails.AppendLine("Topics details:");

                _log.Error(queueDetails, ex);

                throw;
            }
        }

        public void ClearAll()
        {
            // Nasty race condition averted
            var topicMessages = Interlocked.Exchange(ref _topicMessages, new List<BrokeredMessage>());
            foreach (var msg in topicMessages)
            {
                msg.Dispose();
            }
        }
    }
}