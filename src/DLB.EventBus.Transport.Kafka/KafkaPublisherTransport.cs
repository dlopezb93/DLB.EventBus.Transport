using System;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using DLB.EventBus.Transport.Exceptions;
using DLB.EventBus.Transport.Messages;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.Logging;

namespace DLB.EventBus.Transport.Kafka
{
    internal class KafkaPublisherTransport : ITransport
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ILogger _logger;

        public KafkaPublisherTransport(ILogger<KafkaPublisherTransport> logger, IConnectionPool connectionPool)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        }

        public BrokerAddress BrokerAddress => new BrokerAddress("Kafka", _connectionPool.ServersAddress);

        public async Task<OperateResult> SendAsync(TransportMessage message)
        {
            var producer = _connectionPool.RentProducer();

            try
            {
                var headers = new Confluent.Kafka.Headers();

                foreach (var header in message.Headers)
                {
                    headers.Add(header.Value != null
                        ? new Header(header.Key, Encoding.UTF8.GetBytes(header.Value))
                        : new Header(header.Key, null));
                }

                var result = await producer.ProduceAsync(message.GetName(), new Message<string, byte[]>
                {
                    Headers = headers,
                    Key = message.GetId(),
                    Value = message.Body
                });
                
                if (result.Status == PersistenceStatus.Persisted || result.Status == PersistenceStatus.PossiblyPersisted)
                {
                    _logger.LogDebug($"kafka topic message [{message.GetName()}] has been published.");

                    return OperateResult.Success;
                }

                var ex = new PublisherSentFailedException("kafka message persisted failed!");

                return OperateResult.Failed(ex);
            }
            catch (Exception ex)
            {
                var wapperEx = new PublisherSentFailedException(ex.Message, ex);

                return OperateResult.Failed(wapperEx);
            }
            finally
            {
                var returned = _connectionPool.Return(producer);
                if (!returned)
                {
                    producer.Dispose();
                }
            }
        }
    }
}