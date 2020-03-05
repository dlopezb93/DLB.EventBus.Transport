using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// A publish service for publish a message to Transport.
    /// </summary>
    public interface ITransportPublisher
    {
        /// <summary>
        /// Asynchronous publish an object message.
        /// </summary>
        /// <param name="name">the topic name or exchange router key.</param>
        /// <param name="contentObj">message body content, that will be serialized. (can be null)</param>
        /// <param name="cancellationToken"></param>
        Task<OperateResult> PublishAsync<T>(string name, T contentObj, Func<T, string> partitionKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronous publish an object message with custom headers
        /// </summary>
        /// <typeparam name="T">content object</typeparam>
        /// <param name="name">the topic name or exchange router key.</param>
        /// <param name="contentObj">message body content, that will be serialized. (can be null)</param>
        /// <param name="headers">message additional headers.</param>
        /// <param name="cancellationToken"></param>
        Task<OperateResult> PublishAsync<T>(string name, T contentObj, IDictionary<string, string> headers, Func<T, string> partitionKey = null, CancellationToken cancellationToken = default);
    }
}