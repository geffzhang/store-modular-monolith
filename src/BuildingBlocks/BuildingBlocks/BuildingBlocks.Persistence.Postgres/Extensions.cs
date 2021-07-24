using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Persistence;
using BuildingBlocks.Persistence.Postgres.Decorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Persistence.Postgres
{
    public static class Extensions
    {
        public const string SectionName = "postgres";

        internal static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
        {
            var options = configuration.GetSection(sectionName).Get<PostgresOptions>();
            services.AddOptions<PostgresOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();
            services.AddSingleton(new UnitOfWorkTypeRegistry());

            return services;
        }

        public static IServiceCollection AddTransactionalDecorators(this IServiceCollection services)
        {
            services.TryDecorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandlerDecorator<>));
            return services;
        }

        public static IServiceCollection AddPostgres<T>(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName) where T : DbContext
        {
            var options = configuration.GetSection(sectionName).Get<PostgresOptions>();
            services.AddOptions<PostgresOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();
            services.AddDbContext<T>(x => x.UseNpgsql(options.ConnectionString));

            return services;
        }

        public static IServiceCollection AddUnitOfWork<TUnitOfWork, TImplementation>(this IServiceCollection services)
            where TUnitOfWork : class, IUnitOfWork where TImplementation : class, TUnitOfWork
        {
            services.AddScoped<TUnitOfWork, TImplementation>();

            using var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetRequiredService<UnitOfWorkTypeRegistry>().Register<TUnitOfWork>();

            return services;
        }
    }
}