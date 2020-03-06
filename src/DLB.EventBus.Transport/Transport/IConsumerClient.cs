// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using DLB.EventBus.Transport.Messages;

namespace DLB.EventBus.Transport.Transport
{
    /// <inheritdoc />
    /// <summary>
    /// Message queue consumer client
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        BrokerAddress BrokerAddress { get; }

        /// <summary>
        /// Subscribe to a set of topics to the message queue
        /// </summary>
        /// <param name="topics"></param>
        void Subscribe(IEnumerable<string> topics);

        /// <summary>
        /// Start listening
        /// </summary>
        void Listening(CancellationToken cancellationToken);

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        void Commit();

        /// <summary>
        /// Commits the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Commit(object message);

        /// <summary>
        /// Reject message and resumption
        /// </summary>
        void Reject(object sender);

        event EventHandler<TransportMessage> OnMessageReceived;

        /// <summary>
        /// Occurs when [on log error].
        /// </summary>
        event EventHandler<LogMessageEventArgs> OnLogError;

        /// <summary>
        /// Occurs when [on log].
        /// </summary>
        event EventHandler<LogMessageEventArgs> OnLog;
    }
}