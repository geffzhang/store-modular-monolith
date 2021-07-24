using System.Threading.Tasks;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Messaging;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Validations
{
    [Decorator]
    public class MessageValidationDecorator<T> : IMessageHandler<T> where T : class, IMessage
    {
        private readonly IMessageHandler<T> _handler;
        private readonly ILogger<IMessageHandler<T>> _logger;
        private readonly IValidator<T> _validator;

        public MessageValidationDecorator(IMessageHandler<T> handler, ILogger<IMessageHandler<T>> logger,
            IValidator<T> validator)
        {
            _handler = handler;
            _logger = logger;
            _validator = validator;
        }

        public async Task HandleAsync(T message)
        {
            _logger.LogInformation(
                "[{Prefix}] Handle request={X-RequestData}",
                nameof(MessageValidationDecorator<T>), typeof(T).Name);

            _logger.LogDebug($"Handling {typeof(T).FullName} with content {JsonConvert.SerializeObject(message)}");

            await _validator.ValidateAsync(message);

            await _handler.HandleAsync(message);

            _logger.LogInformation($"Handled {typeof(T).FullName}");
        }
    }
}