using PB.ITOps.Messaging.DataProtection;

namespace PB.ITOps.Messaging.PatSender.Encryption
{
    public class EncryptedMessagePublisher: MessagePublisher, IEncryptedMessagePublisher
    {
        public EncryptedMessagePublisher(IMessageSender messageSender, DataProtectionConfiguration configuration, string correlationId = null) 
            : base(messageSender, new EncryptedMessageGenerator(configuration), correlationId)
        {
        }
    }
}
