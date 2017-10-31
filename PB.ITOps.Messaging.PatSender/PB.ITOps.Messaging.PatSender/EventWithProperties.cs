namespace PB.ITOps.Messaging.PatSender
{
    /// <summary>
    /// Represents a pairing of an event with a set of properties specific to that event.
    /// </summary>
    public class EventWithProperties
    {
        /// <summary>
        /// Gets/sets the event object.
        /// </summary>
        public object Event { get; set; }

        /// <summary>
        /// Gets/sets a set of properties associated with the event, used when the event is published.
        /// </summary>
        public MessageProperties Properties { get; set; }
    }
}
