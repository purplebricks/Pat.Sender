using System;
using Microsoft.Azure.ServiceBus;
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
                .SendMessages(Arg.Is<IEnumerable<Message>>(p => 
                p.Any(m => ((string)m.UserProperties["MessageType"]).Equals("PB.ITOps.Messaging.PatSender.UnitTests.Event1"))));
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
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties["MessageType"]).Equals("PB.ITOps.Messaging.PatSender.UnitTests.Event1"))));
        }

        [Fact]
        public async Task WhenPublishEvent_Adds_CustomConstructorProperty()
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
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties[testKey]).Equals(testValue))));
        }

        [Fact]
        public async Task WhenPublishEvent_Adds_CustomMethodProperty()
        {
            var testKey = "testKey";
            var testValue = "test value";
            var customProperties = new Dictionary<string, string>
            {
                {testKey, testValue}
            };
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());
            await messagePublisher.PublishEvent(new Event1(), customProperties);
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties[testKey]).Equals(testValue))));
        }

        [Fact]
        public async Task ScheduleEvent_WhenScheduledEnqueueTimeProvided_ThenMessageIsScheduledForSpecifiedTime()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());
            var enqueueTime = DateTime.UtcNow.AddMinutes(10);

            await messagePublisher.ScheduleEvent(new Event1(), enqueueTime);

            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(t =>
                    t.Any(m => m.ScheduledEnqueueTimeUtc == enqueueTime)));
        }

        [Fact]
        public async Task ScheduleEvents_WhenScheduledEnqueueTimeProvided_ThenAllMessagesAreScheduledForSpecifiedTime()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());
            var enqueueTime = DateTime.UtcNow.AddMinutes(10);
            IEnumerable<object> events = new List<object>
            {
                new Event1(),
                new Event2()
            };

            await messagePublisher.ScheduleEvents(events, enqueueTime);

            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(t =>
                    t.All(m => m.ScheduledEnqueueTimeUtc == enqueueTime)));
        }

        [Fact]
        public async Task SendCommand_WhenSendingCommand_SpecificSubscriberIsSet()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());
            await messagePublisher.SendCommand(new Event1(), "TestSubscriber");
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties["SpecificSubscriber"]).Equals("TestSubscriber"))));
        }

        [Fact]
        public async Task SendCommand_WhenSendingMultipleCommands_SpecificSubscriberIsSetOnAllCommands()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), Guid.NewGuid().ToString());
            await messagePublisher.SendCommands(new[] {new Event1(), new Event1()}, "TestSubscriber");
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Count(m => ((string)m.UserProperties["SpecificSubscriber"]).Equals("TestSubscriber")) == 2));
        }
    }
}
