using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Core.Messaging.Commands
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<T>(T command) where T : class, ICommand
        {
            if (command is null)
            {
                return;
            }

            var scope = _serviceProvider.CreateScope();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);

            await handler.HandleAsync(command);
        }
    }
}