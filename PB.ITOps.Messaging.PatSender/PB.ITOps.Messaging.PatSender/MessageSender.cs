using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.Azure.ServiceBus;
using PB.ITOps.Messaging.PatSender.Extensions;

namespace PB.ITOps.Messaging.PatSender
{
    /// <summary>
    /// Sends brokered messages to specified topic using batching and retry policies.
    /// For internal Pat Sender messaging use only.
    /// </summary>
    public class MessageSender : IMessageSender
    {
        private readonly ILog _log;
        private readonly PatSenderSettings _senderSettings;
        private readonly ConnectionResolver _connectionResolver;

        private const long MaxBatchSizeInBytes = 262144;    // 256k

        public MessageSender(ILog log, PatSenderSettings senderSettings)
        {
            _log = log;
            _senderSettings = senderSettings;
            _connectionResolver = new ConnectionResolver(senderSettings);
        }

        /// <summary>
        /// Sends brokered messages directly to service bus topic
        /// NB: Does not decorate messages with content or message type
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public async Task SendMessages(IEnumerable<Message> messages)
        {
            var messagesToSend = messages.ToList();
            bool retryOnFailOver;
            do
            {
                retryOnFailOver = false;
                var connectionString = _connectionResolver.GetConnection();
                try
                {
                    var client = TopicClientResolver.GetTopic(connectionString, _senderSettings.EffectiveTopicName);
                    await SendPartitionedBatch(client, messagesToSend);
                }
                catch (Exception ex)
                {
                    _log.Warn("Failed sending topic message(s), checking for secondary fail-over service bus...", ex);
                    if (_connectionResolver.HasFailOver())
                    {
                        _log.Info("Failing over to next service bus connection");
                        _connectionResolver.FailOver();
                        retryOnFailOver = true;
                    }
                    else
                    {
                        _log.FatalFormat("Failed to send topic message(s) of type: {0}", string.Join(", ", messagesToSend.Select(m => m.ContentType).Distinct()));
                        throw;
                    }
                }
            } while (retryOnFailOver);
        }

        private async Task SendPartitionedBatch(TopicClient topicClient, IList<Message> brokeredMessageList)
        {
            var batchList = new List<Message>();
            long batchSize = 0;
            var totalMessageCount = brokeredMessageList.Count;

            foreach (var brokeredMessage in brokeredMessageList)
            {
                var size = brokeredMessage.GetSize();
                if ((batchSize + size) > MaxBatchSizeInBytes)
                {
                    // Send current batch
                    await SendBatch(topicClient, batchList, totalMessageCount);

                    // Initialize a new batch
                    batchList = new List<Message> { brokeredMessage };
                    batchSize = size;
                }
                else
                {
                    // Add the BrokeredMessage to the current batch
                    batchList.Add(brokeredMessage);
                    batchSize += size;
                }
            }

            // The final batch is sent outside of the loop
            await SendBatch(topicClient, batchList, totalMessageCount);
        }

        private async Task SendBatch(TopicClient topicClient, List<Message> messages, int totalMessageCount)
        {
            try
            {
                //clone required otherwise retry on failover connection will fail with "brokered message '{id}' has already been consumed"
                var clonedMessages = messages.Select(m => m.Clone()).ToList();
                await topicClient.SendAsync(clonedMessages);                
            }
            catch (Exception exc)
            {
                _log.Warn(totalMessageCount == messages.Count
                    ? $"Failed to send complete batch of messages: {exc.Message}"
                    : $"Failed to send {messages.Count} messages from batch of {totalMessageCount} messages: {exc.Message}");

                throw;
            }
        }
    }
}