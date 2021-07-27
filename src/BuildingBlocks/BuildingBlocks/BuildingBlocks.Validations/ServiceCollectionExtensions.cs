using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Cqrs;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Validations
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ref https://www.jerriepelser.com/blog/validation-response-aspnet-core-webapi
        /// </summary>
        public static async Task HandleValidation<TRequest>(this IValidator<TRequest> validator, TRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.ToValidationResultModel());
            }
        }

        public static IServiceCollection AddCustomValidators(this IServiceCollection services,
            params Assembly[] scanAssemblies)
        {
            var assemblies = scanAssemblies.Any() ? scanAssemblies : AppDomain.CurrentDomain.GetAssemblies();

             services.Scan(scan =>
                scan.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            services.AddScoped(typeof(IMessageMiddleware<>), typeof(MessageValidationMiddleware<>));
            services.AddScoped(typeof(IRequestMiddleware<,>), typeof(CqrsRequestValidatorMiddleware<,>));

            return services;
        }

        private static ValidationResultModel ToValidationResultModel(this ValidationResult validationResult)
        {
            return new(validationResult);
        }
    }
}