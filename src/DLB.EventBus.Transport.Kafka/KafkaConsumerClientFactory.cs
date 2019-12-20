using DLB.EventBus.Transport.Exceptions;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.Options;

namespace DLB.EventBus.Transport.Kafka
{
    /// <summary>
    /// KafkaConsumerClientFactory.
    /// </summary>
    /// <seealso cref="DLB.EventBus.Transport.Kafka.Transport.IConsumerClientFactory" />
    internal sealed class KafkaConsumerClientFactory : IConsumerClientFactory
    {
        private readonly IOptions<KafkaOptions> _kafkaOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumerClientFactory"/> class.
        /// </summary>
        /// <param name="kafkaOptions">The kafka options.</param>
        public KafkaConsumerClientFactory(IOptions<KafkaOptions> kafkaOptions)
        {
            _kafkaOptions = kafkaOptions;
        }

        /// <summary>
        /// Create a new instance of <see cref="T:DLB.EventBus.Transport.Kafka.Transport.IConsumerClient" />.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BrokerConnectionException"></exception>
        public IConsumerClient Create()
        {
            try
            {
                return new KafkaConsumerClient(_kafkaOptions);
            }
            catch (System.Exception e)
            {
                throw new BrokerConnectionException(e);
            }
        }
    }
}