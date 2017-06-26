using System;
using System.Threading;

namespace PB.ITOps.Messaging.PatSender
{
    public class ConnectionResolver
    {
        private readonly PatSenderSettings _senderSettings;
        private static volatile bool _topicUsePrimary;
        private static long _retryPrimaryUtcDateTimeTicks;

        public ConnectionResolver(PatSenderSettings senderSettings)
        {
            _senderSettings = senderSettings;
            _topicUsePrimary = true;
        }

        public void FailOver()
        {
            Interlocked.Exchange(ref _retryPrimaryUtcDateTimeTicks, DateTime.UtcNow.AddMinutes(5).Ticks);
            _topicUsePrimary = false;
        }

        public bool HasFailOver()
        {
            return _topicUsePrimary && string.IsNullOrEmpty(_senderSettings.FailoverConnection);
        }

        public string GetConnection()
        {
            if (_topicUsePrimary)
            {
                return _senderSettings.PrimaryConnection;
            }

            if (ShouldRetestPrimaryConnection())
            {
                _topicUsePrimary = true;
                return _senderSettings.PrimaryConnection;
            }
            return _senderSettings.FailoverConnection;
        }

        private static bool ShouldRetestPrimaryConnection()
        {
            return DateTime.UtcNow > new DateTime(Interlocked.Read(ref _retryPrimaryUtcDateTimeTicks));
        }
    }
}