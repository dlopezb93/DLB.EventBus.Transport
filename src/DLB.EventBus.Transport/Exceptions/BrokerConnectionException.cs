using System;

namespace DLB.EventBus.Transport.Exceptions
{
    /// <summary>
    /// BrokerConnectionException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class BrokerConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerConnectionException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public BrokerConnectionException(Exception innerException)
            : base("Broker Unreachable", innerException)
        {

        }
    } 
}
