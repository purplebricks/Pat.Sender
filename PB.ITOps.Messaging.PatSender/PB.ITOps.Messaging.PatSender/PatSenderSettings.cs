using System;

namespace PB.ITOps.Messaging.PatSender
{
    public class PatSenderSettings
    {
        public PatSenderSettings ()
	    {
            TopicName = "pat";
            UseDevelopmentTopic = true;
	    }

        private string _topicName;

        /// <summary>
        /// Name of topic where messages will be sent
        /// </summary>
        public string TopicName
        {
            get => _topicName;
            set => _topicName = value;
        }

        public string EffectiveTopicName
        {
            get => UseDevelopmentTopic ? _topicName + Environment.MachineName : _topicName;
        }

        /// <summary>
        /// Destination service bus where messages will attempt to be sent 
        /// </summary>
        public string PrimaryConnection { get; set; }

        /// <summary>
        /// optional service bus where messages will be sent if sending to the primary connection fails
        /// </summary>
        public string FailoverConnection { get; set; }

        /// <summary>
        /// If set to true, the machine name will be appended to the topic name
        /// Should be set to true for local development.
        /// </summary>
        public bool UseDevelopmentTopic { get; set; }
    }
}