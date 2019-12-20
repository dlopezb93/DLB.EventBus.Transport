using DLB.EventBus.Transport;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.EventBus.Transport.HostedServices
{
    /// <summary>
    /// EventBusHostedService.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class EventBusHostedService : BackgroundService
    {
        private const string HandleAsyncMethod = "HandleAsync";

        private readonly IConsumerClient _consumerClient;
        private readonly IEnumerable<ISubscriber> _subscribers;
        private TransportOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBusHostedService"/> class.
        /// </summary>
        /// <param name="consumerClientFactory">The consumer client factory.</param>
        /// <param name="subscribers">The subscribers.</param>
        /// <param name="transportOptions">The transport options.</param>
        /// <exception cref="ArgumentNullException">subscribers</exception>
        public EventBusHostedService(
                                    IConsumerClientFactory consumerClientFactory,
                                    IEnumerable<ISubscriber> subscribers,
                                    IOptions<TransportOptions> transportOptions)
        {
            _subscribers = subscribers ?? throw new ArgumentNullException(nameof(subscribers));
            _options = transportOptions.Value;

            //TODO think groups id
            _consumerClient = consumerClientFactory.Create();
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.
        /// </returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                // Subscribe to topics.
                var topics = _subscribers.Select(p => p.Topic);

                _consumerClient.Subscribe(topics);
                _consumerClient.OnMessageReceived += OnMessageReceived;
                _consumerClient.OnLogError += OnLogErrorReceived;
                _consumerClient.OnLog += OnLogReceived;

                _consumerClient.Listening(stoppingToken);
            });            

            return Task.CompletedTask;
        }        

        private void OnMessageReceived(object sender, DLB.EventBus.Transport.Messages.TransportMessage e)
        {
            var handler = _subscribers.FirstOrDefault(p => p.Topic == e.Topic);
            string jsonStr = Encoding.UTF8.GetString(e.Body);

            if (!string.IsNullOrEmpty(jsonStr))
            {
                // Message received context
                var context = new HandleContext(_consumerClient, e.Headers);
                var method = handler.GetType().GetMethod(HandleAsyncMethod);

                // Get only dynamic parameter, type of IntegrationEventData
                var type = method.GetParameters().FirstOrDefault().ParameterType;
                var data = JsonConvert.DeserializeObject(jsonStr, type);

                object[] parametersArray = new object[] { data, context, this };

                method.Invoke(handler, parametersArray);
            }
        }

        private void OnLogErrorReceived(object sender, LogMessageEventArgs e)
        {
            _options.RaiseLogError(sender, e);
        }

        private void OnLogReceived(object sender, LogMessageEventArgs e)
        {
            _options.RaiseLog(sender, e);
        }
    }
}
