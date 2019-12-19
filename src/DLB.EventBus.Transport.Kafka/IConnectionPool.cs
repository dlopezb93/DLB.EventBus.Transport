using Confluent.Kafka;

namespace DLB.EventBus.Transport.Kafka
{
    public interface IConnectionPool
    {
        string ServersAddress { get; }

        IProducer<string, byte[]> RentProducer();

        bool Return(IProducer<string, byte[]> producer);
    }
}