using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Core;
using BuildingBlocks.Cqrs.Commands;
using BuildingBlocks.Cqrs.Events;
using BuildingBlocks.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Cqrs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] scanAssemblies)
        {
            var assemblies = scanAssemblies.Any() ? scanAssemblies : AppDomain.CurrentDomain.GetAssemblies();

            services.AddScoped(typeof(IRequestProcessor<,>), typeof(RequestProcessor<,>));
            services.AddScoped<IMediator, Mediator>();

            ScanForCqrsHandlers(services, assemblies);
            AddRequestMiddlewares(services, assemblies);

            return services;
        }

        private static IServiceCollection AddRequestMiddlewares(IServiceCollection services, IEnumerable<Assembly>
            assembliesToScan)
        {
            services.Scan(s => s.FromAssemblies(assembliesToScan)
                .AddClasses(c => c.AssignableTo(typeof(IRequestMiddleware<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static void ScanForCqrsHandlers(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = (assembliesToScan as Assembly[] ?? assembliesToScan).Distinct().ToArray();

            AddCommand(services, assembliesToScan.ToList());
            AddEvent(services, assembliesToScan.ToList());
            AddQuery(services, assembliesToScan.ToList());
        }

        private static IServiceCollection AddCommand(IServiceCollection services, IList<Assembly> assemblies)
        {
            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddEvent(IServiceCollection services, IList<Assembly> assemblies)
        {
            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<,>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddQuery(IServiceCollection services, IList<Assembly> assemblies)
        {
            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}