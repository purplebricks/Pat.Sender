using System.Threading.Tasks;
using PB.ITOps.Messaging.PatSender.Send;

namespace PB.ITOps.Messaging.PatSender.Publish
{
    // <summary>
    /// Simple message publishes which will immediately send any published messages
    // </summary>
    public class MessagePublisher : BaseMessagePublisher
    {
        public MessagePublisher(IClientBusQueue clientBusQueue): base(clientBusQueue)
        {
        }

        protected override async Task PostQueuedAction()
        {
            await ClientBusQueue.SendAll();
        }
    }
}