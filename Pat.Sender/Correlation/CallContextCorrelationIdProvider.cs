#if NETSTANDARD2_0
using System.Collections.Concurrent;
using System.Threading;
#else
using System.Runtime.Remoting.Messaging;
#endif

namespace Pat.Sender.Correlation
{
    public class CallContextCorrelationIdProvider : ICorrelationIdProvider
    {
        private const string Key = "CorrelationId";
#if NETSTANDARD2_0
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();
        public string CorrelationId => state.TryGetValue(Key, out AsyncLocal<object> data) ? (string)data.Value : null;
#else
        public string CorrelationId => (string)CallContext.LogicalGetData(Key);
#endif
    }
}