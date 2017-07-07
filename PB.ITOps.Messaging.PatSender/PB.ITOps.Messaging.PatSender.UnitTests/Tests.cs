using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using NSubstitute;
using Xunit;

namespace PB.ITOps.Messaging.PatSender.UnitTests
{
    public class Event1
    {
        
    }

    public class Event2
    {

    }

    public class MessagePublisherTests
    {
        [Fact]
        public async Task WhenPublishMessage_MessageTypeIsFullEventName()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender);
            await messagePublisher.PublishEvent(new Event1());
            messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<BrokeredMessage>>(p => 
                p.Any(m => ((string)m.Properties["MessageType"]).Equals("PB.ITOps.Messaging.PatSender.UnitTests.Event1"))));
        }

        [Fact]
        public async Task WhenPublishMessages_MessageTypeIsFullEventName()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender);

            IEnumerable<object> events = new List<object>
            {
                new Event1(),
                new Event2()
            };
            await messagePublisher.PublishEvents(events);

            messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<BrokeredMessage>>(p =>
                    p.Any(m => ((string)m.Properties["MessageType"]).Equals("PB.ITOps.Messaging.PatSender.UnitTests.Event1"))));
        }
    }
}
