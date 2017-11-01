using PB.ITOps.Messaging.DataProtection;

namespace PB.ITOps.Messaging.PatSender.Encryption
{
    public class EncryptedMessagePublisher: MessagePublisher, IEncryptedMessagePublisher
    {
        public EncryptedMessagePublisher(IMessageSender messageSender, DataProtectionConfiguration configuration, MessageProperties defaultMessageProperties) 
            : base(messageSender, new EncryptedMessageGenerator(configuration), defaultMessageProperties)
        {
        }
    }
}