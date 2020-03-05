using Microsoft.Extensions.DependencyInjection;
using System;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection" /> for configuring consistence services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ServiceCollection;

        /// <summary>
        /// Adds and configures the consistence services for the consistency.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="TransportOptions" />.</param>
        /// <returns>An <see cref="TransportBuilder" /> for application services.</returns>
        public static TransportBuilder AddTransport(this IServiceCollection services, Action<TransportOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            ServiceCollection = services;

            services.AddTransient<ITransportPublisher, TransportPublisher>();

            //Options and extension service
            var options = new TransportOptions();

            setupAction(options);

            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }

            services.Configure(setupAction);

            return new TransportBuilder(services);
        }

    }
}