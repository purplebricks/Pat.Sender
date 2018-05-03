using NSubstitute;
using Pat.Sender.Correlation;
using Pat.Sender.Extensions;
using Pat.Sender.MessageGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Xunit;

namespace Pat.Sender.UnitTests
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
            var messagePublisher = CreatePublisher(messageSender);
            await messagePublisher.PublishEvent(new Event1());
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p => 
                p.Any(m => ((string)m.UserProperties["MessageType"]).Equals("Pat.Sender.UnitTests.Event1"))));
        }

        [Fact]
        public async Task WhenPublishEvents_MessageTypeIsFullEventName()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = CreatePublisher(messageSender);

            IEnumerable<object> events = new List<object>
            {
                new Event1(),
                new Event2()
            };
            await messagePublisher.PublishEvents(events);

            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties["MessageType"]).Equals("Pat.Sender.UnitTests.Event1"))));
        }

        [Fact]
        public async Task WhenPublishEvent_Adds_CustomConstructorProperty()
        {
            const string testKey = "testKey";
            const string testValue = "test value";
            var customProperties = new Dictionary<string, string>
            {
                {testKey, testValue}
            };
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = new MessagePublisher(messageSender, new MessageGenerator(), new MessageProperties(new NewCorrelationIdProvider()) { CustomProperties = customProperties });
            await messagePublisher.PublishEvent(new Event1());
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties[testKey]).Equals(testValue))));
        }

        [Fact]
        public async Task WhenPublishEvent_Adds_CustomMethodProperty()
        {
            const string testKey = "testKey";
            const string testValue = "test value";
            var customProperties = new Dictionary<string, string>
            {
                {testKey, testValue}
            };
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = CreatePublisher(messageSender);
            await messagePublisher.PublishEvent(new Event1(), new MessageProperties(new NewCorrelationIdProvider()) { CustomProperties = customProperties });
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties[testKey]).Equals(testValue))));
        }

        [Fact]
        public async Task WhenPublishEventsWithProperties_Adds_CustomProperty()
        {
            const string testKey = "testKey";
            const string testValue = "test value";
            var customProperties = new Dictionary<string, string>
            {
                {testKey, testValue}
            };

            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = CreatePublisher(messageSender);
            await messagePublisher.PublishEventsWithProperties(
                new[]
                {
                    new EventWithProperties
                    {
                        Event = new Event1(),
                        Properties = new MessageProperties(
                            new NewCorrelationIdProvider())
                        {
                            CustomProperties = customProperties
                        }
                    }
                });

            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties[testKey]).Equals(testValue))));
        }

        [Fact]
        public async Task WhenPublishEventsWithProperties_SetsCorrelationId()
        {
            const string correlationId = "AAA";

            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = CreatePublisher(messageSender);
            await messagePublisher.PublishEventsWithProperties(
                new[]
                {
                    new EventWithProperties
                    {
                        Event = new Event1(),
                        Properties = new MessageProperties(new LiteralCorrelationIdProvider(correlationId))
                    }
                });

            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => m.GetCorrelationId().Equals(correlationId))));
        }

        [Fact]
        public async Task ScheduleEvent_WhenScheduledEnqueueTimeProvided_ThenMessageIsScheduledForSpecifiedTime()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = CreatePublisher(messageSender);
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
            var messagePublisher = CreatePublisher(messageSender);
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
            var messagePublisher = CreatePublisher(messageSender);
            await messagePublisher.SendCommand(new Event1(), "TestSubscriber");
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Any(m => ((string)m.UserProperties["SpecificSubscriber"]).Equals("TestSubscriber"))));
        }

        [Fact]
        public async Task SendCommand_WhenSendingMultipleCommands_SpecificSubscriberIsSetOnAllCommands()
        {
            var messageSender = Substitute.For<IMessageSender>();
            var messagePublisher = CreatePublisher(messageSender);
            await messagePublisher.SendCommands(new[] {new Event1(), new Event1()}, "TestSubscriber");
            await messageSender.Received(1)
                .SendMessages(Arg.Is<IEnumerable<Message>>(p =>
                    p.Count(m => ((string)m.UserProperties["SpecificSubscriber"]).Equals("TestSubscriber")) == 2));
        }

        private static MessagePublisher CreatePublisher(IMessageSender messageSender)
            => new MessagePublisher(messageSender, new MessageGenerator(), new MessageProperties(new NewCorrelationIdProvider()));
    }
}
