using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Persistence.MSSQL
{
    public static class Extensions
    {
        private const string SectionName = "mssql";

        public static IServiceCollection AddMssqlPersistence<TContext>(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName, Action<IServiceCollection> configurator = null)
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
            }))
            .AddScoped<ISqlDbContext>(ctx => ctx.GetRequiredService<TContext>())
            .AddScoped<IDomainEventContext>(ctx => ctx.GetRequiredService<TContext>());

            services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<ISqlDbContext>());

            configurator?.Invoke(services);

            return services;
        }

        public static async ValueTask<TResponse> HandleTransaction<TDbContext, TResponse>(this ICommandProcessor commandProcessor,
            TDbContext dbContext, CancellationToken cancellationToken, Func<Task<TResponse>> next)
            where TDbContext : DbContext, ISqlDbContext, IDomainEventContext
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // Achieving atomicity
               await dbContext.BeginTransactionAsync();

                var response = await next();
                var domainEvents = dbContext.GetDomainEvents().ToList();

                var tasks = domainEvents
                    .Select(async @event =>
                    {
                        var id = (response as dynamic)?.Id;
                        @event.Id = id;

                        // publish it out
                        await commandProcessor.PublishDomainEventAsync(@event);
                    });

                await Task.WhenAll(tasks);
                await dbContext.CommitTransactionAsync();
                
                return response;
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