using DLB.EventBus.Transport.Transport;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            DefaultGroup = "transport.group." + Assembly.GetEntryAssembly()?.GetName().Name.ToLower();
            Extensions = new List<ITransportOptionsExtension>();
        }        

        internal IList<ITransportOptionsExtension> Extensions { get; }

        /// <summary>
        /// Gets the default group.
        /// </summary>
        /// <value>
        /// The default group.
        /// </value>
        public string DefaultGroup { get; set; }

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