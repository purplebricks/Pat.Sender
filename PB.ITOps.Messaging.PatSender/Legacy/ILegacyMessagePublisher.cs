using System.Threading.Tasks;

namespace PB.ITOps.Messaging.PatSender.Legacy
{
    public interface ILegacyMessagePublisher
    {
        Task PublishLegacyMessage<TLegacyMessage>(TLegacyMessage message, string legacyMessageType, string legacyContentType) where TLegacyMessage : class;
    }
}
