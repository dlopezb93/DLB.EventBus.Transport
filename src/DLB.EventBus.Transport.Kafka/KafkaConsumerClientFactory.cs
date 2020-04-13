using DLB.EventBus.Transport.Exceptions;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DLB.EventBus.Transport.Kafka
{
    /// <summary>
    /// KafkaConsumerClientFactory.
    /// </summary>
    /// <seealso cref="DLB.EventBus.Transport.Kafka.Transport.IConsumerClientFactory" />
    internal sealed class KafkaConsumerClientFactory : IConsumerClientFactory
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly ILogger<KafkaLog> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumerClientFactory"/> class.
        /// </summary>
        /// <param name="kafkaOptions">The kafka options.</param>
        public KafkaConsumerClientFactory(KafkaOptions kafkaOptions, ILogger<KafkaLog> logger)
        {
            _kafkaOptions = kafkaOptions;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create a new instance of <see cref="T:DLB.EventBus.Transport.Kafka.Transport.IConsumerClient" />.
        /// </summary>
        /// <param name="groupId">message group number</param>
        /// <returns></returns>
        /// <exception cref="BrokerConnectionException"></exception>
        public IConsumerClient Create(string groupId)
        {
            try
            {
                return new KafkaConsumerClient(groupId, _kafkaOptions, _logger);
            }
            catch (System.Exception e)
            {
                throw new BrokerConnectionException(e);
            }
        }
    }
}