using DLB.EventBus.Transport.Messages;
using DLB.EventBus.Transport.Transport;
using System;
using System.Collections.Generic;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// HandleContext.
    /// </summary>
    /// <seealso cref="DLB.EventBus.Transport.IHandleContext" />
    public class HandleContext : IHandleContext
    {
        private readonly IConsumerClient _consumerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleContext"/> class.
        /// </summary>
        /// <param name="consumerClient">The consumer client.</param>
        /// <param name="headers">The headers.</param>
        /// <exception cref="ArgumentNullException">consumerClient</exception>
        public HandleContext(IConsumerClient consumerClient, TransportMessage message)
        {
            _consumerClient = consumerClient ?? throw new ArgumentNullException(nameof(consumerClient));
            MessageContext = new MessageContext(message);
        }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public MessageContext MessageContext { get; }

        /// <summary>
        /// Commits the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Commit()
        {
            _consumerClient.Commit();
        }

        public void Commit(object message)
        {
            _consumerClient.Commit(message);
        }
    }
}
