using System;

namespace DLB.EventBus.Transport.Exceptions
{
    /// <summary>
    /// PublisherSentFailedException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class PublisherSentFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSentFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PublisherSentFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSentFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public PublisherSentFailedException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}