using Pat.Sender.Correlation;
using System.Collections.Generic;

namespace Pat.Sender
{
    /// <summary>
    /// Represents metadata associated with a message.
    /// </summary>
    public class MessageProperties
    {
        /// <summary>
        /// Constructs a new instance of <see cref="MessageProperties"/>.
        /// </summary>
        /// <param name="correlationIdProvider">Source of correlation ids for the messages that these properties are associated with.</param>
        public MessageProperties(ICorrelationIdProvider correlationIdProvider)
        {
            CorrelationIdProvider = correlationIdProvider;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="MessageProperties"/> using a <see cref="LiteralCorrelationIdProvider"/>.
        /// </summary>
        /// <param name="correlationId">The literal correlation id to use for the associated event(s).</param>
        /// <remarks>
        /// This overload is used to support a simpler IoC setup when publishing a set of events and correlation ids using the
        /// <see cref="EventWithProperties"/> pairing.</remarks>
        public MessageProperties(string correlationId)
        {
            CorrelationIdProvider = new LiteralCorrelationIdProvider(correlationId);
        }

        /// <summary>
        /// A dictionary of custom properties that are set against the published event's message, or null to avoid setting any.
        /// </summary>
        public IDictionary<string, string> CustomProperties { get; set; }

        /// <summary>
        /// Gets the source of correlation ids for the messages that these properties are associated with.
        /// </summary>
        public ICorrelationIdProvider CorrelationIdProvider { get; }
    }
}
