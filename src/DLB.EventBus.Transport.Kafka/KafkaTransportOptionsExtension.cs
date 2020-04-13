using DLB.EventBus.Transport.HostedServices;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DLB.EventBus.Transport.Kafka
{
    /// <summary>
    /// KafkaTransportOptionsExtension.
    /// </summary>
    /// <seealso cref="DLB.EventBus.Transport.ITransportOptionsExtension" />
    internal sealed class KafkaTransportOptionsExtension : ITransportOptionsExtension
    {
        private readonly Action<KafkaOptions> _configure;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaTransportOptionsExtension"/> class.
        /// </summary>
        /// <param name="configure">The configure.</param>
        public KafkaTransportOptionsExtension(Action<KafkaOptions> configure)
        {
            _configure = configure;
        }

        /// <summary>
        /// Registered child service.
        /// </summary>
        /// <param name="services">add service to the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /></param>
        public void AddServices(IServiceCollection services)
        {
            var options = new KafkaOptions();

            _configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton<ITransport, KafkaPublisherTransport>();
            services.AddSingleton<IConsumerClientFactory, KafkaConsumerClientFactory>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();           

            services.AddHostedService<EventBusHostedService>();
        }
    }
}