using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DLB.EventBus.Transport;
using Sample.Transport.AspNetCore.Kafka.IntegrationEventsHandlers;

namespace Sample.Transport.AspNetCore.Kafka
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get settings.
            var eventBusSettings = Configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>();

            services.AddTransport(opt =>
            {
                opt.UseKafka(cnf =>
                {
                    // If you wish define your custom kafka settings
                    //cnf.MainConfig = new Confluent.Kafka.ClientConfig(new Dictionary<string, string>());
                    cnf.MainConfig.BootstrapServers = eventBusSettings.Servers;
                    cnf.MainConfig.SslCaLocation = eventBusSettings.SSLCeriticatePath;
                });

                opt.DefaultGroup = "default_group"; // Optional
                opt.OnLog += Transport_OnLog;
                opt.OnLogError += Transport_OnLogError;

            }).RegisterSubscriber<HelloWorldIntegrationEventHandler>()
              .RegisterSubscriber<HelloNewSchemaIntegrationEventHandler>();

            services.AddControllers();
        }

        private void Transport_OnLogError(object sender, DLB.EventBus.Transport.Transport.LogMessageEventArgs e)
        {
            Console.WriteLine(e.Reason);
        }

        private void Transport_OnLog(object sender, DLB.EventBus.Transport.Transport.LogMessageEventArgs e)
        {
            Console.WriteLine(e.Reason);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
