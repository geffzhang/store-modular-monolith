using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Core;
using BuildingBlocks.Cqrs.Commands;
using BuildingBlocks.Cqrs.Events;
using BuildingBlocks.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Cqrs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] scanAssemblies)
        {
            var assemblies = scanAssemblies.Any() ? scanAssemblies : AppDomain.CurrentDomain.GetAssemblies();

            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped(typeof(IRequestProcessor<,>), typeof(RequestProcessor<,>));
            services.AddScoped<IMediator, Mediator>();

            services.AddScoped<ServiceFactoryDelegate>(p => (type =>
            {
                try
                {
                    return p.GetService(type);
                }
                catch (ArgumentException)
                {
                    // Let's assume it's a constrained generic type
                    if (type.IsConstructedGenericType &&
                        type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var serviceType = type.GenericTypeArguments.Single();
                        var serviceTypes = new List<Type>();
                        foreach (var service in services)
                        {
                            if (serviceType.IsConstructedGenericType &&
                                serviceType.GetGenericTypeDefinition() == service.ServiceType)
                            {
                                try
                                {
                                    var closedImplType =
                                        service.ImplementationType.MakeGenericType(serviceType.GenericTypeArguments);
                                    serviceTypes.Add(closedImplType);
                                }
                                catch
                                {
                                }
                            }
                        }

                        services.Replace(new ServiceDescriptor(type,
                            sp => { return serviceTypes.Select(sp.GetService).ToArray(); }, ServiceLifetime.Transient));

                        var resolved = Array.CreateInstance(serviceType, serviceTypes.Count);

                        Array.Copy(serviceTypes.Select(p.GetService).ToArray(), resolved, serviceTypes.Count);

                        return resolved;
                    }

                    throw;
                }
            }));

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