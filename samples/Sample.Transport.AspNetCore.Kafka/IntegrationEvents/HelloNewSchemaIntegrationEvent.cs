namespace Sample.Transport.AspNetCore.Kafka.IntegrationEvents
{
    public class HelloNewSchemaIntegrationEvent
    {
        public string CustomerName { get; set; }
        public int Counter { get; set; }
    }
}
