using System.Threading.Tasks;
using PB.ITOps.Messaging.PatSender.Send;

namespace PB.ITOps.Messaging.PatSender.Publish
{
    /// <summary>
    /// Publishes batches of messages to the local message queue
    /// NB: requires explicit ClientBusQueue.SendAll() to complete the action and actually send the locally queued messages
    /// </summary>
    public class BatchMessagePublisher : BaseMessagePublisher
    {
        public BatchMessagePublisher(IClientBusQueue clientBusQueue) : base(clientBusQueue)
        {
        }

        protected override Task PostQueuedAction()
        {
            return Task.CompletedTask;
        }
    }
}