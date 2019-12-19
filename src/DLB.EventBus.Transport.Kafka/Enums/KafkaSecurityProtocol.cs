namespace DLB.EventBus.Transport.Kafka.Enums
{
    public class KafkaSecurityProtocol
    {
        public static KafkaSecurityProtocol SSL = new KafkaSecurityProtocol("SSL");

        public static KafkaSecurityProtocol PlainText = new KafkaSecurityProtocol("PLAINTEXT");

        public static KafkaSecurityProtocol SalslPlainText = new KafkaSecurityProtocol("SASL_PLAINTEXT");

        public static KafkaSecurityProtocol SaslSSL = new KafkaSecurityProtocol("SASL_SSL");

        private KafkaSecurityProtocol(string protocol)
        {
            Protocol = protocol;
        }

        public string Protocol { get; }
    }
}
