using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using DLB.EventBus.Transport.Messages;
using DLB.EventBus.Transport.Transport;
using Microsoft.Extensions.Options;

namespace DLB.EventBus.Transport.Kafka
{
    /// <summary>
    /// KafkaConsumerClient.
    /// </summary>
    /// <seealso cref="DLB.EventBus.Transport.Kafka.Transport.IConsumerClient" />
    internal sealed class KafkaConsumerClient : IConsumerClient
    {
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private readonly string _groupId;
        private readonly KafkaOptions _kafkaOptions;
        private IConsumer<string, byte[]> _consumerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumerClient"/> class.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public KafkaConsumerClient(IOptions<KafkaOptions> options)
        {
            _kafkaOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
            _groupId = _kafkaOptions.GroupId ?? "group_id";
        }

        /// <summary>
        /// Occurs when [on message received].
        /// </summary>
        public event EventHandler<TransportMessage> OnMessageReceived;

        /// <summary>
        /// Occurs when [on log error].
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnLogError;

        /// <summary>
        /// Occurs when [on log].
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnLog;

        /// <summary>
        /// Gets the broker address.
        /// </summary>
        /// <value>
        /// The broker address.
        /// </value>
        public BrokerAddress BrokerAddress => new BrokerAddress("Kafka", _kafkaOptions.Servers);

        /// <summary>
        /// Subscribe to a set of topics to the message queue
        /// </summary>
        /// <param name="topics"></param>
        /// <exception cref="ArgumentNullException">topics</exception>
        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            Connect();

            _consumerClient.Subscribe(topics);
        }

        /// <summary>
        /// Start listening
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Listening(CancellationToken cancellationToken)
        {
            Connect();

            while (true)
            {
                var consumerResult = _consumerClient.Consume(cancellationToken);

                if (consumerResult.IsPartitionEOF || consumerResult.Value == null) continue;

                var headers = new Dictionary<string, string>(consumerResult.Headers.Count);
                foreach (var header in consumerResult.Headers)
                {
                    var val = header.GetValueBytes();
                    headers.Add(header.Key, val != null ? Encoding.UTF8.GetString(val) : null);
                }

                headers.Add(Messages.Headers.Group, _groupId);

                if (_kafkaOptions.CustomHeaders != null)
                {
                    var customHeaders = _kafkaOptions.CustomHeaders(consumerResult);
                    foreach (var customHeader in customHeaders)
                    {
                        headers.Add(customHeader.Key, customHeader.Value);
                    }
                }

                var message = new TransportMessage(headers, consumerResult.Value, consumerResult.Topic);

                OnMessageReceived?.Invoke(consumerResult, message);
            }
        }

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        /// <param name="sender"></param>
        public void Commit(object sender)
        {
            _consumerClient.Commit((ConsumeResult<string, byte[]>)sender);
        }

        /// <summary>
        /// Reject message and resumption
        /// </summary>
        /// <param name="sender"></param>
        public void Reject(object sender)
        {
            _consumerClient.Assign(_consumerClient.Assignment);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _consumerClient?.Dispose();
        }

        #region private methods

        private void Connect()
        {
            if (_consumerClient != null)
            {
                return;
            }

            _connectionLock.Wait();

            try
            {
                if (_consumerClient == null)
                {
                    _kafkaOptions.MainConfig["group.id"] = _groupId;
                    _kafkaOptions.MainConfig["auto.offset.reset"] = "earliest";
                    var config = _kafkaOptions.AsKafkaConfig();

                    _consumerClient = new ConsumerBuilder<string, byte[]>(config)
                        .SetErrorHandler(ConsumerClient_OnConsumeError)
                        .SetLogHandler(ConsumerClient_OnLog)
                        .Build();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private void ConsumerClient_OnLog(IConsumer<string, byte[]> arg1, LogMessage arg2)
        {
            var logArgs = new LogMessageEventArgs
            {
                LogType = MqLogType.ConsumerRegistered,
                Reason = $"{arg2.Message}"
            };

            OnLog?.Invoke(null, logArgs);
        }

        private void ConsumerClient_OnConsumeError(IConsumer<string, byte[]> consumer, Error e)
        {
            var logArgs = new LogMessageEventArgs
            {
                LogType = MqLogType.ServerConnError,
                Reason = $"An error occurred during connect kafka --> {e.Reason}"
            };
            OnLogError?.Invoke(null, logArgs);
        }

        #endregion private methods
    }
}