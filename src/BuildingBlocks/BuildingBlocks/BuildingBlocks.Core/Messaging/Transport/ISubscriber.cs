﻿using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Transport
{
    public interface ISubscriber
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}