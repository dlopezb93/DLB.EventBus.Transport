// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DLB.EventBus.Transport.Messages;
using System.Threading.Tasks;

namespace DLB.EventBus.Transport.Transport
{
    public interface ITransport
    {
        BrokerAddress BrokerAddress { get; }

        Task<OperateResult> SendAsync(TransportMessage message);
    }
}
