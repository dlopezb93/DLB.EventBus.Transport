using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Confluent.Kafka;
using DLB.EventBus.Transport.Kafka.Enums;

namespace DLB.EventBus.Transport.Kafka
{
    /// <summary>
    /// Provides programmatic configuration for the Transport kafka project.
    /// </summary>
    public class KafkaOptions
    {        
        private IEnumerable<KeyValuePair<string, string>> _kafkaConfig;

        public KafkaOptions()
        {
            MainConfig = new ConcurrentDictionary<string, string>();
            SecurityProtocol = KafkaSecurityProtocol.PlainText;
        }

        /// <summary>
        /// librdkafka configuration parameters (refer to https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md).
        /// <para>
        /// Topic configuration parameters are specified via the "default.topic.config" sub-dictionary config parameter.
        /// </para>
        /// </summary>
        public readonly ConcurrentDictionary<string, string> MainConfig;

        /// <summary>
        /// Producer connection pool size, default is 10
        /// </summary>
        public int ConnectionPoolSize { get; set; } = 10;

        /// <summary>
        /// The `bootstrap.servers` item config of <see cref="MainConfig" />.
        /// <para>
        /// Initial list of brokers as a CSV list of broker host or host:port.
        /// </para>
        /// </summary>
        public string Servers { get; set; }

        /// <summary>
        /// Group id from service.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the security protocol.
        /// </summary>
        /// <value>
        /// The security protocol.
        /// </value>
        public KafkaSecurityProtocol SecurityProtocol { get; set; }

        /// <summary>
        /// Gets or sets the SSL certificate path.
        /// </summary>
        /// <value>
        /// The SSL certificate path.
        /// </value>
        public string SSLCertificatePath { get; set; }

        /// <summary>
        /// If you need to get offset and partition and so on.., you can use this function to write additional header into <see cref="TransportHeader"/>
        /// </summary>
        public Func<ConsumeResult<string, byte[]>, List<KeyValuePair<string, string>>> CustomHeaders { get; set; }

        internal IEnumerable<KeyValuePair<string, string>> AsKafkaConfig()
        {
            if (_kafkaConfig == null)
            {
                if (string.IsNullOrWhiteSpace(Servers))
                {
                    throw new ArgumentNullException(nameof(Servers));
                }

                MainConfig["debug"] = "consumer, broker";
                MainConfig["bootstrap.servers"] = Servers;
                MainConfig["queue.buffering.max.ms"] = "10";
                MainConfig["enable.auto.commit"] = "false";
                MainConfig["log.connection.close"] = "true";
                MainConfig["request.timeout.ms"] = "3000";
                MainConfig["message.timeout.ms"] = "5000";
                MainConfig["reconnect.backoff.ms"] = "5000";        
                MainConfig["retry.backoff.ms"] = "50";                          
                MainConfig["security.protocol"] = SecurityProtocol.Protocol;                

                if (!string.IsNullOrEmpty(SSLCertificatePath))
                {
                    if (!File.Exists(SSLCertificatePath))
                    {
                        throw new FileNotFoundException(SSLCertificatePath);
                    }

                    MainConfig["ssl.ca.location"] = SSLCertificatePath;
                }

                _kafkaConfig = MainConfig.AsEnumerable();
            }

            return _kafkaConfig;
        }
    }
}