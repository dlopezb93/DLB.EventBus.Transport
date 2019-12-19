using DLB.EventBus.Transport;
using DLB.EventBus.Transport.Kafka;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TransportOptionsExtensions
    {
        /// <summary>
        /// Configuration to use kafka in Transport.
        /// </summary>
        /// <param name="options">Transport configuration options</param>
        /// <param name="bootstrapServers">Kafka bootstrap server urls.</param>
        public static TransportOptions UseKafka(this TransportOptions options, string bootstrapServers)
        {
            return options.UseKafka(opt => { opt.Servers = bootstrapServers; });
        }

        /// <summary>
        /// Configuration to use kafka in Transport.
        /// </summary>
        /// <param name="options">Transport configuration options</param>
        /// <param name="configure">Provides programmatic configuration for the kafka .</param>
        /// <returns></returns>
        public static TransportOptions UseKafka(this TransportOptions options, Action<KafkaOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new KafkaTransportOptionsExtension(configure));

            return options;
        }
    }
}