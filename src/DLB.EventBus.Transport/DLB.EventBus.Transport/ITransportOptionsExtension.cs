using Microsoft.Extensions.DependencyInjection;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// Transport options extension
    /// </summary>
    public interface ITransportOptionsExtension
    {
        /// <summary>
        /// Registered child service.
        /// </summary>
        /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
        void AddServices(IServiceCollection services);
    }
}