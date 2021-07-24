using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Messaging.Commands
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
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }
    }
}