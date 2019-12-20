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
                    cnf.Servers = eventBusSettings.Servers;
                    cnf.SSLCertificatePath = eventBusSettings.SSLCeriticatePath;
                });

                opt.DefaultGroup = "default_group"; // Optional
                opt.OnLog += Transport_OnLog;
                opt.OnLogError += Transport_OnLogError;
            });

            services.AddControllers();
        }

        private void Transport_OnLogError(object sender, DLB.EventBus.Transport.Transport.LogMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Transport_OnLog(object sender, DLB.EventBus.Transport.Transport.LogMessageEventArgs e)
        {
            throw new NotImplementedException();
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
