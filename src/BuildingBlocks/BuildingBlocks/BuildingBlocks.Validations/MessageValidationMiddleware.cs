using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Validations
{
    public class MessageValidationMiddleware<TMessage> : IMessageMiddleware<TMessage> where TMessage : class, IMessage
    {
        private readonly ILogger<MessageValidationMiddleware<TMessage>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MessageValidationMiddleware(ILogger<MessageValidationMiddleware<TMessage>> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync(TMessage message, IMessageContext messageContext,
            CancellationToken cancellationToken,
            HandleMessageDelegate<TMessage> next)
        {
            var validator = _serviceProvider.GetService<IValidator<TMessage>>();
            if (validator is null)
            {
                await next(message, messageContext, cancellationToken);
                return;
            }

            _logger.LogInformation(
                "[{Prefix}] Handle message={X-MessageData}",
                nameof(MessageValidationMiddleware<TMessage>), typeof(TMessage).Name);

            _logger.LogDebug($"Handling {typeof(TMessage).FullName} with content {JsonConvert.SerializeObject(message)}");

            await validator.ValidateAsync(message, cancellationToken);

            await next(message, messageContext, cancellationToken);

            _logger.LogInformation($"Handled {typeof(TMessage).FullName}");
        }
    }
}