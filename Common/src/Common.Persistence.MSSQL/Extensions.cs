using Common.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Persistence.MSSQL
{
    public static class Extensions
    {
        private const string SectionName = "mssql";

        public static IServiceCollection AddMSSQLPersistence(this IServiceCollection services, string sectionName = SectionName)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISqlDbContext>(ctx => ctx.GetRequiredService<SqlDbContext>());

            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var mssqlOptions = services.GetOptions<MSSQLOptions>(sectionName);

            services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(mssqlOptions.ConnectionString));
            return services;
        }
    }
}