using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DLB.EventBus.Transport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Transport.AspNetCore.Kafka.IntegrationEvents;

namespace Sample.Transport.AspNetCore.Kafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {       
        private readonly ILogger<HelloWorldController> _logger;
        private readonly ITransportPublisher transportPublisher;

        public HelloWorldController(
                               ILogger<HelloWorldController> logger,
                               ITransportPublisher transportPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.transportPublisher = transportPublisher ?? throw new ArgumentNullException(nameof(transportPublisher));
        }

        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var integrationEvent = new HelloWorldIntegrationEvent()
            {
                Id = id,
                Value = "Hello world!",
            };

            await this.transportPublisher.PublishAsync("hello_world", integrationEvent);

            return "Message sended!";
        }
    }
}
