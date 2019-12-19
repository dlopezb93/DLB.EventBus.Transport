namespace DLB.EventBus.Transport
{
    //
    // Resumen:
    //     Marker interface used to assist identification in IoC containers. Not to be used
    //     directly as it does not contain the message type of the consumer
    //
    // Comentarios:
    //     Not to be used directly by application code, for internal reflection only
    public interface ISubscriber
    {
        /// <summary>
        /// Gets the topic.
        /// </summary>
        /// <value>
        /// The topic.
        /// </value>
        string Topic { get; }
    }
}
