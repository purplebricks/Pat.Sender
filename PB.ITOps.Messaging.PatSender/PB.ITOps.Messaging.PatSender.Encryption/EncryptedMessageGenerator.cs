using Microsoft.AspNetCore.DataProtection;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using PB.ITOps.Messaging.DataProtection;
using PB.ITOps.Messaging.PatSender.MessageGeneration;

namespace PB.ITOps.Messaging.PatSender.Encryption
{
    public class EncryptedMessageGenerator: IMessageGenerator
    {
        private readonly IDataProtector _dataProtector;

        public EncryptedMessageGenerator(DataProtectionConfiguration configuration)
        {
            var provider = DataProtection.DataProtectionProvider.Create(configuration);
            _dataProtector = provider.CreateProtector("PatLite");
        }
        
        public BrokeredMessage GenerateBrokeredMessage(object message)
        {
            var messageBody = JsonConvert.SerializeObject(message);
            var protectedmessageBody = _dataProtector.Protect(messageBody);
            var brokeredMessage = new BrokeredMessage(protectedmessageBody);
            brokeredMessage.Properties.Add("Encrypted", true);
            return brokeredMessage;
        }
    }
}