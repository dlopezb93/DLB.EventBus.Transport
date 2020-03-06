using DLB.EventBus.Transport.Messages;
using System.Collections.Generic;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// IHandleContext.
    /// </summary>
    public interface IHandleContext
    {
        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        MessageContext MessageContext { get; }

        /// <summary>
        /// Commits the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        void Commit();

        /// <summary>
        /// Commits the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Commit(object message);
    }
}
