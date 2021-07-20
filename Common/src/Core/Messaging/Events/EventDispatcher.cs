﻿using System;
using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Core.Messaging.Events
{
    public class






        EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<T>(T @event) where T : class, IEvent
        {
            if (@event is null)
            {
                return;
            }

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(@event.GetType());
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            await handler.HandleAsync(@event);
        }
    }
}