using System;
using Xunit;

namespace Pat.Sender.UnitTests
{
    public class PatSenderSettingsTests
    {
        [Fact]
        public void PatSenderSettings_DoesNotAppendMachineNameToTopic_WhenNotInDevelopmentMode()
        {
            var configuration = new PatSenderSettings
            {
                TopicName = "test",
                UseDevelopmentTopic = false
            };

            Assert.Equal("test", configuration.EffectiveTopicName);
        }

        [Fact]
        public void PatSenderSettings_DoesAppendMachineNameToTopic_WhenInDevelopmentMode()
        {
            var configuration = new PatSenderSettings
            {
                TopicName = "test",
                UseDevelopmentTopic = true
            };

            Assert.Equal("test" + Environment.MachineName, configuration.EffectiveTopicName);
        }

        [Fact]
        public void PatSenderSettings_EntersDevelopmentModeByDefault()
        {
            var configuration = new PatSenderSettings();
            Assert.True(configuration.UseDevelopmentTopic);
        }
    }
}
