namespace PB.ITOps.Messaging.PatSender
{
    public class PatSenderSettings
    {
        public string TopicName { get; set; }
        public string PrimaryConnection { get; set; }
        public string FailoverConnection { get; set; }
    }
}