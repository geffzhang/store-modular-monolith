using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext>();

            builder.UseSqlServer(
                "Data Source=.\\sqlexpress;Initial Catalog=OnlineStore;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30");
            return new IdentityDbContext(builder.Options);
        }
    }
}