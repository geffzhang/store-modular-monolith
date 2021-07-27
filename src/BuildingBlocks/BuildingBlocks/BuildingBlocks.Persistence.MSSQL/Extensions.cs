using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Persistence;
using BuildingBlocks.Cqrs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Persistence.MSSQL
{
    public static class Extensions
    {
        private const string SectionName = "mssql";

        public static IServiceCollection AddMssqlPersistence<TContext>(this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = SectionName,
            Action<IServiceCollection> configurator = null,
            Action<DbContextOptionsBuilder> optionBuilder = null)
            where TContext : DbContext, ISqlDbContext, IDomainEventContext, IDbFacadeResolver
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            services.Configure<MssqlOptions>(configuration.GetSection(sectionName));
            var mssqlOptions = configuration.GetSection(sectionName).Get<MssqlOptions>();

            services.AddDbContext<TContext>(options => options.UseSqlServer(mssqlOptions.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    //adding all existing triggers dynamically   
                    options.UseTriggers(triggerOptions =>
                        triggerOptions.AddAssemblyTriggers(typeof(TContext).Assembly));
                    optionBuilder?.Invoke(options);
                }))
                .AddScoped<ISqlDbContext>(ctx => ctx.GetRequiredService<TContext>())
                .AddScoped<IDomainEventContext, DomainEventContext>()
                .AddScoped<IDbFacadeResolver>(ctx => ctx.GetRequiredService<TContext>());


            services.AddScoped(typeof(IRequestMiddleware<,>), typeof(TransactionalCqrsRequestMiddleware<,>));

            configurator?.Invoke(services);

            return services;
        }

        public static async Task HandleTransactionAsync<TDbContext>(
            this ICommandProcessor commandProcessor,
            TDbContext dbContext,
            IList<IDomainEvent> events,
            Func<Task> next)
            where TDbContext : ISqlDbContext
        {
            //https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            //https://devblogs.microsoft.com/cesardelatorre/using-resilient-entity-framework-core-sql-connections-and-transactions-retries-with-exponential-backoff/
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var isInnerTransaction = dbContext.Database.CurrentTransaction is not null;
                var transaction = dbContext.Database.CurrentTransaction ??
                                  await dbContext.Database.BeginTransactionAsync();


                await next();
                var domainEvents = events == null || events.Any() == false
                    ? dbContext.GetDomainEvents().ToList()
                    : events;
                var tasks = domainEvents.Select(async @event =>
                {
                    // also will publish our domain event notification internally
                    await commandProcessor.PublishDomainEventAsync(@event);
                });

                await Task.WhenAll(tasks);
                if (isInnerTransaction == false)
                    await transaction.CommitAsync();
            });
        }

        public static IServiceCollection AddRepository(this IServiceCollection services, Type repoType)
        {
            services.Scan(scan => scan
                .FromAssembliesOf(repoType)
                .AddClasses(classes =>
                    classes.AssignableTo(repoType)).As(typeof(IRepository<,,>)).WithScopedLifetime()
            );

            return services;
        }

        public static void MigrateDataFromScript(this MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetCallingAssembly();
            var files = assembly.GetManifestResourceNames();
            var filePrefix = $"{assembly.GetName().Name}.Data.Scripts."; //IMPORTANT

            foreach (var file in files
                .Where(f => f.StartsWith(filePrefix) && f.EndsWith(".sql"))
                .Select(f => new {PhysicalFile = f, LogicalFile = f.Replace(filePrefix, string.Empty)})
                .OrderBy(f => f.LogicalFile))
            {
                using var stream = assembly.GetManifestResourceStream(file.PhysicalFile);
                using var reader = new StreamReader(stream!);
                var command = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(command))
                    continue;

                migrationBuilder.Sql(command);
            }
        }
    }
}