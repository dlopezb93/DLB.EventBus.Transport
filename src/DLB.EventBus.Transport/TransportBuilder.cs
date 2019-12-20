using Microsoft.Extensions.DependencyInjection;
using System;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// Allows fine grained configuration of Transport services.
    /// </summary>
    public sealed class TransportBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="ArgumentNullException">services</exception>
        public TransportBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the <see cref="IServiceCollection" /> where MVC services are configured.
        /// </summary>
        public IServiceCollection Services { get; }        

        public TransportBuilder RegisterSubscriber<TEventHandler>()
            where TEventHandler : ISubscriber
        {
            return AddTransient(typeof(ISubscriber), typeof(TEventHandler));
        }

        /// <summary>
        /// Adds a scoped service of the type specified in serviceType with an implementation
        /// </summary>
        private TransportBuilder AddTransient(Type serviceType, Type concreteType)
        {
            Services.AddTransient(serviceType, concreteType);
            return this;
        }
    }
}