using System.Threading.Tasks;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// An empty interface, which is used to mark the current class have a Transport subscriber methods.
    /// </summary>
    public interface ITransportSubscriber<in TIntegrationEvent> : ISubscriber
        where TIntegrationEvent : class
    {
        /// <summary>
        /// Handles the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="consumer">The consumer.</param>
        /// <param name="sender">The sender.</param>
        /// <returns></returns>
        Task HandleAsync(TIntegrationEvent context, IHandleContext consumer, object sender);
    }   
}