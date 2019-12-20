using DLB.EventBus.Transport;
using Microsoft.Extensions.Logging;
using Sample.Transport.AspNetCore.Kafka.IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace Sample.Transport.AspNetCore.Kafka.IntegrationEventsHandlers
{
    public class HelloWorldIntegrationEventHandler : ITransportSubscriber<HelloWorldIntegrationEvent>
    {
        private readonly ILogger<HelloWorldIntegrationEventHandler> logger;

        public HelloWorldIntegrationEventHandler(ILogger<HelloWorldIntegrationEventHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Topic => "hello_world";

        public async Task HandleAsync(HelloWorldIntegrationEvent @event, IHandleContext context, object sender)
        {
            this.logger.LogInformation($"New integration event received! value: '{@event.Value}'");

            this.logger.LogInformation("----------Message metadata----------");

            this.logger.LogInformation($"Id: {context.MessageContext.Id}");
            this.logger.LogInformation($"SentTime: {context.MessageContext.SentTime}");
            this.logger.LogInformation($"Name: {context.MessageContext.Name}");
            this.logger.LogInformation($"Group: {context.MessageContext.Group}");            

            // Any operation
            await Task.Delay(TimeSpan.FromSeconds(5));

            this.logger.LogInformation("Set message as commited...");

            context.Commit(sender);
        }
    }
}
