using System.Threading.Tasks;
using Common.Core;
using Common.Core.Extensions;
using Common.Core.Messaging.Commands;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Common.Logging.Serilog
{
    [Decorator]
    internal sealed class LoggingCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly ILogger<ICommandHandler<T>> _logger;

        public LoggingCommandHandlerDecorator(ICommandHandler<T> handler,
            ILogger<ICommandHandler<T>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task HandleAsync(T command)
        {
            var module = command.GetModuleName();
            var name = command.GetType().Name.Underscore();
            _logger.LogInformation($"Handling a command: '{name}' ('{module}')...");
            await _handler.HandleAsync(command);
            _logger.LogInformation($"Completed handling a command: '{name}' ('{module}').");
        }
    }
}