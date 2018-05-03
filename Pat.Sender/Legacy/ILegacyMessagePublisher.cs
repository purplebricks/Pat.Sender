using System.Threading.Tasks;

namespace Pat.Sender.Legacy
{
    public interface ILegacyMessagePublisher
    {
        Task PublishLegacyMessage<TLegacyMessage>(TLegacyMessage message, string legacyMessageType, string legacyContentType) where TLegacyMessage : class;
    }
}
