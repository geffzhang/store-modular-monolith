using System;
using System.Threading.Tasks;
using Common.Web.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Commands
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceFactory;

        public CommandDispatcher(IServiceProvider serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task SendAsync<T>(T command) where T : class, ICommand
        {
            if (command is null)
            {
                return;
            }

            using var scope = _serviceFactory.CreateScope();
            if (command.CorrelationId == Guid.Empty)
            {
                var context = scope.ServiceProvider.GetRequiredService<ICorrelationContextAccessor>();
                command.CorrelationId = Guid.Parse(context.CorrelationContext.CorrelationId);
            }

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);

            await handler.HandleAsync(command);
        }
    }
}