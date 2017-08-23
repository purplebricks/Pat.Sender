using System;
using Microsoft.ServiceBus.Messaging;
using NSubstitute;
using PB.ITOps.Messaging.PatSender.MessageGeneration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task WhenPublishEvent_MessageTypeIsFullEventName()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());
            await messagePublisher.PublishEvent(new Event1());
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<BrokeredMessage>>(p => 
                p.Any(m => ((string)m.Properties["MessageType"]).Equals("PB.ITOps.Messaging.PatSender.UnitTests.Event1"))));
        }

        [Fact]
        public async Task WhenPublishEvents_MessageTypeIsFullEventName()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());

            IEnumerable<object> events = new List<object>
            {
                new Event1(),
                new Event2()
            };
            await messagePublisher.PublishEvents(events);

            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<BrokeredMessage>>(p =>
                    p.Any(m => ((string)m.Properties["MessageType"]).Equals("PB.ITOps.Messaging.PatSender.UnitTests.Event1"))));
        }

        [Fact]
        public async Task WhenPublishEvent_Adds_CustomProperty()
        {
            var testKey = "testKey";
            var testValue = "test value";
            var customProperties = new Dictionary<string, string>
            {
                {testKey, testValue}
            };
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString(), customProperties);
            await messagePublisher.PublishEvent(new Event1());
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<BrokeredMessage>>(p =>
                    p.Any(m => ((string)m.Properties[testKey]).Equals(testValue))));
        }
    }
}
