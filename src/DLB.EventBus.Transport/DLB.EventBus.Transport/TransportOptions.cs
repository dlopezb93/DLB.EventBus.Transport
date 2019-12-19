using DLB.EventBus.Transport.Transport;
using System;
using System.Collections.Generic;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// Represents all the options you can use to configure the system.
    /// </summary>
    public class TransportOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportOptions"/> class.
        /// </summary>
        public TransportOptions()
        {
            Extensions = new List<ITransportOptionsExtension>();
        }        

        internal IList<ITransportOptionsExtension> Extensions { get; }

        /// <summary>
        /// Occurs when [on log error].
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnLogError;

        /// <summary>
        /// Occurs when [on log].
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnLog;
       
        internal void RaiseLogError(object sender, LogMessageEventArgs e)
        {
            OnLogError?.Invoke(sender, e);
        }

        internal void RaiseLog(object sender, LogMessageEventArgs e)
        {
            OnLog?.Invoke(sender, e);
        }

        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(ITransportOptionsExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extensions.Add(extension);
        }
    }
}