using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;

namespace Pat.Sender
{
    public static class TopicClientResolver
    {
        private static readonly ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();
        private static readonly Dictionary<string, TopicClient> Clients = new Dictionary<string, TopicClient>();
        private static readonly Dictionary<string, TopicClient> TokenClients = new Dictionary<string, TopicClient>();

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

                var topicClient = new TopicClient(connectionString, topicName, RetryPolicy.Default);
                Clients.Add(connectionString, topicClient);
                return topicClient;
            }
        }

        public static TopicClient GetTopic(string connectionString, string topicName, ITokenProvider tokenProvider)
        {
            if (TokenClients.ContainsKey(connectionString))
            {
                return TokenClients[connectionString];
            }

            lock (Locks.GetOrAdd(connectionString, new object()))
            {
                if (TokenClients.ContainsKey(connectionString))
                {
                    return TokenClients[connectionString];
                }
                ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(connectionString);
                var topicClient = new TopicClient(builder.Endpoint, topicName, tokenProvider, retryPolicy: RetryPolicy.Default);
                TokenClients.Add(connectionString, topicClient);
                return topicClient;
            }
        }
    }
}
