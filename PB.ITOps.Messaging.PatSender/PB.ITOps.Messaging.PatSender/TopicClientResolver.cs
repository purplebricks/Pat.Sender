using System;
using System.Collections.Concurrent;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender
{
    public class TopicClientResolver
    {
        private static readonly ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();
        private static readonly ConcurrentDictionary<string, TopicClient> Clients = new ConcurrentDictionary<string, TopicClient>();

        public static TopicClient GetTopic(string connectionString, string topicName)
        {
            if (Clients.ContainsKey(connectionString))
            {
                return Clients[connectionString];
            }

            lock (Locks.GetOrAdd(connectionString, new object()))
            {
                if (Clients.ContainsKey(connectionString))
                {
                    return Clients[connectionString];
                }

                var topicClient = TopicClient.CreateFromConnectionString(connectionString, topicName);
                if (!Clients.TryAdd(connectionString, topicClient))
                {
                    throw new InvalidOperationException($"Failed to add new topic client for topic {topicName}");
                }
                return topicClient;
            }
        }
    }
}
