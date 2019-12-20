using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.IO;

namespace DLB.EventBus.Transport.Kafka
{
    /// <summary>
    /// Provides programmatic configuration for the Transport kafka project.
    /// </summary>
    public class KafkaOptions
    {        
        //private IEnumerable<KeyValuePair<string, string>> _kafkaConfig;

        public KafkaOptions()
        {
            MainConfig = new ClientConfig();

            SetDefaultConfiguration();
        }

        public ClientConfig MainConfig { get; set; }

        /// <summary>
        /// Producer connection pool size, default is 10
        /// </summary>
        public int ConnectionPoolSize { get; set; } = 10;       
       
        /// <summary>
        /// If you need to get offset and partition and so on.., you can use this function to write additional header into <see cref="TransportHeader"/>
        /// </summary>
        public Func<ConsumeResult<string, byte[]>, List<KeyValuePair<string, string>>> CustomHeaders { get; set; }


        internal IEnumerable<KeyValuePair<string, string>> AsKafkaConfig()
        {
            if (string.IsNullOrWhiteSpace(MainConfig.BootstrapServers))
            {
                throw new ArgumentNullException(nameof(MainConfig.BootstrapServers));
            }

            if (!string.IsNullOrEmpty(MainConfig.SslCaLocation))
            {
                if (!File.Exists(MainConfig.SslCaLocation))
                {
                    throw new FileNotFoundException(MainConfig.SslCaLocation);
                }
            }

            return MainConfig;
        }

        private void SetDefaultConfiguration()
        {
            MainConfig.SecurityProtocol = SecurityProtocol.Plaintext;

            MainConfig.Debug = "consumer, broker";
            MainConfig.Set("queue.buffering.max.ms","10");
            MainConfig.Set("enable.auto.commit","false");
            MainConfig.LogConnectionClose = true;
            MainConfig.Set("request.timeout.ms", "3000");
            MainConfig.Set("message.timeout.ms", "5000");
            MainConfig.Set("reconnect.backoff.ms", "5000");
            MainConfig.Set("retry.backoff.ms", "50");
        }
    }
}