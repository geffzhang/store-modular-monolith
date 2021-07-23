using System.Threading.Tasks;
using Common.Core;
using Common.Core.Messaging.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Validations
{
    [Decorator]
    public class CommandValidationDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly ILogger<ICommandHandler<T>> _logger;
        private readonly IValidator<T> _validator;

        public CommandValidationDecorator(ICommandHandler<T> handler, ILogger<ICommandHandler<T>> logger,
            IValidator<T> validator)
        {
            _handler = handler;
            _logger = logger;
            _validator = validator;
        }

        public async Task HandleAsync(T command)
        {
            _logger.LogInformation(
                "[{Prefix}] Handle request={X-RequestData}",
                nameof(CommandValidationDecorator<T>), typeof(T).Name);

            _logger.LogDebug($"Handling {typeof(T).FullName} with content {JsonConvert.SerializeObject(command)}");

            await _validator.ValidateAsync(command);

            await _handler.HandleAsync(command);

            _logger.LogInformation($"Handled {typeof(T).FullName}");
        }
    }
}