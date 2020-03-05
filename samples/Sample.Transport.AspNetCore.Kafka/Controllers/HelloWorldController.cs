using DLB.EventBus.Transport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Transport.AspNetCore.Kafka.IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace Sample.Transport.AspNetCore.Kafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
        private const string helloWorldTopic = "hellow_world";

        private readonly ILogger<HelloWorldController> _logger;
        private readonly ITransportPublisher _transportPublisher;

        public HelloWorldController(
                               ILogger<HelloWorldController> logger,
                               ITransportPublisher transportPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _transportPublisher = transportPublisher ?? throw new ArgumentNullException(nameof(transportPublisher));
        }

        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var integrationEvent = new HelloWorldIntegrationEvent()
            {
                Id = id,
                Value = "Hello world!",
            };

            // Send mensaje to Kafka with specific partition key.
            var result = await _transportPublisher.PublishAsync(helloWorldTopic, integrationEvent, p => p.Id);

            return $"Success: {result.Succeeded}";
        }

        [HttpGet("second/{id}")]
        public async Task<string> GetSecondSchema(string name)
        {
            var integrationEvent = new HelloNewSchemaIntegrationEvent()
            {
                CustomerName = name,
                Counter = 1,
            };

            // Send mensaje to Kafka without specific partition key.
            var result = await _transportPublisher.PublishAsync(helloWorldTopic, integrationEvent);

            return $"Success: {result.Succeeded}";
        }
    }
}
