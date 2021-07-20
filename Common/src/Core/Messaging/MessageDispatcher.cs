using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Core.Messaging
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<T>(T message) where T : class, IMessage
        {
            if (message is null)
            {
                return;
            }
            var handler = _serviceProvider.GetRequiredService<IMessageHandler<T>>();
            await handler?.HandleAsync(message);
            // var handlerType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
            // dynamic handler = _serviceProvider.GetRequiredService(handlerType);
            // await handler.HandleAsync(message);
        }
    }
}