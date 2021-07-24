using System;
using System.Reflection;
using System.Threading.Tasks;
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
            params Assembly[] assemblies)
        {
            return services.Scan(scan =>
                scan.FromAssemblies(assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
                    .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
        }

        private static ValidationResultModel ToValidationResultModel(this ValidationResult validationResult)
        {
            return new(validationResult);
        }
    }
}