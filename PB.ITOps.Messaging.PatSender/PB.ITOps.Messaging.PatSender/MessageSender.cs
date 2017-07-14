using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.ServiceBus;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;

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
        private readonly RetryPolicy<ServiceBusTransientErrorDetectionStrategy> _retryPolicy;
        private readonly ConnectionResolver _connectionResolver;

        private const long MaxBatchSizeInBytes = 262144;    // 256k

        public MessageSender(ILog log, PatSenderSettings senderSettings)
        {
            _log = log;
            _senderSettings = senderSettings;
            _retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(RetryStrategy.DefaultClientRetryCount);
            _retryPolicy.Retrying += _retryPolicy_Retrying;
            _connectionResolver = new ConnectionResolver(senderSettings);
        }

        /// <summary>
        /// Sends brokered messages directly to service bus topic
        /// NB: Does not decorate messages with content or message type
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public async Task SendMessages(IEnumerable<BrokeredMessage> messages)
        {
            var messagesToSend = messages.ToList();
            TopicClient client = null;
            bool retryOnFailOver;
            do
            {
                retryOnFailOver = false;
                var connectionString = _connectionResolver.GetConnection();
                try
                {
                    client = TopicClient.CreateFromConnectionString(connectionString, _senderSettings.TopicName);
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
                finally
                {
                    if (client != null)
                    {
                        client.Close();
                    }
                }
            } while (retryOnFailOver);
        }

        private async Task SendPartitionedBatch(TopicClient topicClient, IList<BrokeredMessage> brokeredMessageList)
        {
            var batchList = new List<BrokeredMessage>();
            long batchSize = 0;
            var totalMessageCount = brokeredMessageList.Count;

            foreach (var brokeredMessage in brokeredMessageList)
            {
                if ((batchSize + brokeredMessage.Size) > MaxBatchSizeInBytes)
                {
                    // Send current batch
                    await SendBatch(topicClient, batchList, batchSize, totalMessageCount);

                    // Initialize a new batch
                    batchList = new List<BrokeredMessage> { brokeredMessage };
                    batchSize = brokeredMessage.Size;
                }
                else
                {
                    // Add the BrokeredMessage to the current batch
                    batchList.Add(brokeredMessage);
                    batchSize += brokeredMessage.Size;
                }
            }

            // The final batch is sent outside of the loop
            await SendBatch(topicClient, batchList, batchSize, totalMessageCount);
        }

        private async Task SendBatch(TopicClient topicClient, List<BrokeredMessage> messages, long batchSize, int totalMessageCount)
        {
            try
            {
                //clone required within retry policy, otherwise retry will fail with "brokered message '{id}' has already been consumed"
                await _retryPolicy.ExecuteAction(async () => await topicClient.SendBatchAsync(messages.Select(m => m.Clone()).ToList()));
            }
            catch (Exception exc)
            {
                _log.Warn(batchSize == messages.Count
                    ? $"Failed to send complete batch of messages: {exc.Message}"
                    : $"Failed to send {batchSize} messages from batch of {totalMessageCount} messages: {exc.Message}");

                throw;
            }
        }

        private void _retryPolicy_Retrying(object sender, RetryingEventArgs e)
        {
            _log.Info($"MessageSender failed to send, attempting to retry because of {e.LastException.Message}");
        }
    }
}