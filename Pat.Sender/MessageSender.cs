using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Pat.Sender.Extensions;

namespace Pat.Sender
{
    /// <summary>
    /// Sends brokered messages to specified topic using batching and retry policies.
    /// For internal Pat Sender messaging use only.
    /// </summary>
    public class MessageSender : IMessageSender
    {
        private readonly IPatSenderLog _log;
        private readonly PatSenderSettings _senderSettings;
        private readonly ConnectionResolver _connectionResolver;

        private const long MaxBatchSizeInBytes = 262144;    // 256k

        public MessageSender(IPatSenderLog<MessageSender> log, PatSenderSettings senderSettings)
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
                    _log.LogWarning("Failed sending topic message(s), checking for secondary fail-over service bus...", ex);
                    if (_connectionResolver.HasFailOver())
                    {
                        _log.LogInformation("Failing over to next service bus connection");
                        _connectionResolver.FailOver();
                        retryOnFailOver = true;
                    }
                    else
                    {
                        _log.LogCritical($"Failed to send topic message(s) of type: {string.Join(", ", messagesToSend.Select(m => m.ContentType).Distinct())}");
                        throw;
                    }
                }
            } while (retryOnFailOver);
        }

        private async Task SendPartitionedBatch(TopicClient topicClient, IList<Message> messageList)
        {
            var batchList = new List<Message>();
            long batchSize = 0;
            var totalMessageCount = messageList.Count;

            foreach (var message in messageList)
            {
                var size = message.GetEstimatedMessageSize();
                if (batchSize + size > MaxBatchSizeInBytes)
                {
                    // Send current batch
                    await SendBatch(topicClient, batchList, totalMessageCount);

                    // Initialize a new batch
                    batchList = new List<Message> { message };
                    batchSize = size;
                }
                else
                {
                    // Add the Message to the current batch
                    batchList.Add(message);
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
                _log.LogWarning(totalMessageCount == messages.Count
                    ? $"Failed to send complete batch of messages: {exc.Message}"
                    : $"Failed to send {messages.Count} messages from batch of {totalMessageCount} messages: {exc.Message}");

                throw;
            }
        }
    }
}