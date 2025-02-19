﻿using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging
{
    public interface IMessageMiddleware<TMessage> where TMessage : IMessage
    {
        Task RunAsync(TMessage message, IMessageContext messageContext, CancellationToken cancellationToken,
            HandleMessageDelegate<TMessage> next);
    }
}