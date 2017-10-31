using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace PB.ITOps.Messaging.PatSender
{
    public static class TopicClientResolver
    {
        private static readonly ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();
        private static readonly Dictionary<string, TopicClient> Clients = new Dictionary<string, TopicClient>();

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
                topicClient.RetryPolicy = RetryPolicy.Default;
                Clients.Add(connectionString, topicClient);
                return topicClient;
            }
        }
    }
}
