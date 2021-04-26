using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
    {
        public SecurityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SecurityDbContext>();

            builder.UseSqlServer(
                "Data Source=.\\sqlexpress;Initial Catalog=OnlineStore;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30");
            builder.UseOpenIddict();
            return new SecurityDbContext(builder.Options);
        }
    }
}