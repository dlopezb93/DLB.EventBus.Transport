using System;
using System.Collections.Generic;

namespace DLB.EventBus.Transport.Messages
{
    /// <summary>
    /// MessageContext.
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContext"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <exception cref="ArgumentNullException">headers</exception>
        public MessageContext(IDictionary<string, string> headers)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get
            {
                Headers.TryGetValue(Messages.Headers.MessageId, out var value);
                return value;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                Headers.TryGetValue(Messages.Headers.MessageName, out var value);
                return value;
            }
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string Group
        {
            get
            {
                Headers.TryGetValue(Messages.Headers.Group, out var value);
                return value;
            }
        }

        /// <summary>
        /// Gets the sent time.
        /// </summary>
        /// <value>
        /// The sent time.
        /// </value>
        public string SentTime
        {
            get
            {
                Headers.TryGetValue(Messages.Headers.SentTime, out var value);
                return value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has exception.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has exception; otherwise, <c>false</c>.
        /// </value>
        public bool HasException => Headers.ContainsKey(Messages.Headers.Exception);
    }
}