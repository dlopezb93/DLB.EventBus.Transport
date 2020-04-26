using DLB.EventBus.Transport;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
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
                                    IServiceScopeFactory serviceScopeFactory,
                                    IOptions<TransportOptions> transportOptions)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _options = transportOptions.Value;

            //TODO think groups id
            _consumerClient = consumerClientFactory.Create(_options.DefaultGroup);
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
                IEnumerable<string> topics;
                using(var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscribers = scope.ServiceProvider.GetService<IEnumerable<ISubscriber>>();
                    topics = subscribers?.Select(p => p.Topic);
                }

                // Subscribe to topics.
                _consumerClient.Subscribe(topics);
                _consumerClient.OnMessageReceived += async (s, m) => await OnMessageReceivedAsync(s, m);
                _consumerClient.OnLogError += OnLogErrorReceived;
                _consumerClient.OnLog += OnLogReceived;

                _consumerClient.Listening(stoppingToken);
            });            

            return Task.CompletedTask;
        }        

        private async Task OnMessageReceivedAsync(object sender, DLB.EventBus.Transport.Messages.TransportMessage e)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var subscribers = scope.ServiceProvider.GetService<IEnumerable<ISubscriber>>();
                string jsonStr = Encoding.UTF8.GetString(e.Body);
                var handlerList = subscribers.Where(p => p.Topic == e.Topic);

                if (!string.IsNullOrEmpty(jsonStr) && handlerList != null && handlerList.Any())
                {
                    // Message received context
                    var context = new HandleContext(_consumerClient, e);

                    foreach (var handler in handlerList)
                    {
                        // Get only dynamic parameter, type of IntegrationEventData
                        var method = handler.GetType().GetMethod(HandleAsyncMethod);
                        var type = method.GetParameters().FirstOrDefault().ParameterType;
                        var headerType = e.Headers[Messages.Headers.Type];


                        if (headerType == type.Name)
                        {
                            // If no throw exception we found a valid handler
                            var data = JsonConvert.DeserializeObject(jsonStr, type, this._options.JsonSerializerSettings);

                            object[] parametersArray = new object[] { data, context, this };

                            var task = (Task)method.Invoke(handler, parametersArray);
                            
                            await task.ConfigureAwait(false);

                            break;
                        }
                    }
                }
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
