using DLB.EventBus.Transport;
using DLB.EventBus.Transport.Messages;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.EventBus.Transport
{
    /// <summary>
    /// TransportPublisher.
    /// </summary>
    /// <seealso cref="DLB.EventBus.Transport.ITransportPublisher" />
    public class TransportPublisher : ITransportPublisher
    {
        private ITransport _transport;
        private TransportOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportPublisher"/> class.
        /// </summary>
        /// <param name="transport">The transport.</param>
        /// <exception cref="ArgumentNullException">transport</exception>
        public TransportPublisher(
                ITransport transport,
                IOptions<TransportOptions> transportOptions)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _options = transportOptions.Value;
        }

        /// <summary>
        /// Asynchronous publish an object message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">the topic name or exchange router key.</param>
        /// <param name="contentObj">message body content, that will be serialized. (can be null)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<OperateResult> PublishAsync<T>(string name, T contentObj,Func<T, string> partitionKey = null, CancellationToken cancellationToken = default)
        {
            return Publish(name, contentObj, null, partitionKey);
        }

        /// <summary>
        /// Asynchronous publish an object message with custom headers
        /// </summary>
        /// <typeparam name="T">content object</typeparam>
        /// <param name="name">the topic name or exchange router key.</param>
        /// <param name="contentObj">message body content, that will be serialized. (can be null)</param>
        /// <param name="headers">message additional headers.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<OperateResult> PublishAsync<T>(string name, T contentObj, IDictionary<string, string> headers, Func<T, string> partitionKey = null, CancellationToken cancellationToken = default)
        {
            return Publish(name, contentObj, headers, partitionKey);
        }

        private Task<OperateResult> Publish<T>(string name, T value, IDictionary<string, string> headers, Func<T, string> partitionKey = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var messageId = partitionKey != null ? partitionKey(value) : Guid.NewGuid().ToString();
            headers.Add(Headers.MessageId, messageId);
            headers.Add(Headers.MessageName, name);
            headers.Add(Headers.Type, typeof(T).Name);
            headers.Add(Headers.SentTime, DateTimeOffset.Now.ToString());

            if (!headers.ContainsKey(Headers.CorrelationId))
            {
                headers.Add(Headers.CorrelationId, messageId);
            }

            var json = JsonConvert.SerializeObject(value, _options.JsonSerializerSettings);
            var bytes = Encoding.UTF8.GetBytes(json);

            var message = new TransportMessage(headers, bytes, name);

            
            return _transport.SendAsync(message);
        }
    }
}
