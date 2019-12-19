using System;
using System.Collections.Generic;

namespace DLB.EventBus.Transport.Messages
{
    /// <summary>
    /// Message content field
    /// </summary>
    public class TransportMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportMessage"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <param name="topic">The topic.</param>
        /// <exception cref="ArgumentNullException">headers</exception>
        public TransportMessage(IDictionary<string, string> headers, byte[] body, string topic)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Body = body;
            Topic = topic;
        }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        /// <value>
        /// The topic.
        /// </value>
        public string Topic { get; set; }

        /// <summary>
        /// Gets the headers of this message
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the body object of this message
        /// </summary>
        public byte[] Body { get; }

        public string GetId()
        {
            return Headers.TryGetValue(Messages.Headers.MessageId, out var value) ? value : null;
        }

        public string GetName()
        {
            return Headers.TryGetValue(Messages.Headers.MessageName, out var value) ? value : null;
        }

        public string GetGroup()
        {
            return Headers.TryGetValue(Messages.Headers.Group, out var value) ? value : null;
        }
    }
}
