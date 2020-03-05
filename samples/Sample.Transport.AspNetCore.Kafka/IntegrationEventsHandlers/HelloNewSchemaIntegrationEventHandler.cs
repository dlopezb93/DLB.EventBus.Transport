using DLB.EventBus.Transport;
using Microsoft.Extensions.Logging;
using Sample.Transport.AspNetCore.Kafka.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Transport.AspNetCore.Kafka.IntegrationEventsHandlers
{
    public class HelloNewSchemaIntegrationEventHandler : ITransportSubscriber<HelloNewSchemaIntegrationEvent>
    {
        private readonly ILogger<HelloNewSchemaIntegrationEventHandler> logger;

        public HelloNewSchemaIntegrationEventHandler(ILogger<HelloNewSchemaIntegrationEventHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Topic => "hello_world";

        public Task HandleAsync(HelloNewSchemaIntegrationEvent @event, IHandleContext context, object sender)
        {
            this.logger.LogInformation($"Handling {nameof(HelloNewSchemaIntegrationEvent)}");

            context.Commit();

            return Task.CompletedTask;
        }
    }
}
