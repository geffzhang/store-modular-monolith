using Common.Messaging.Outbox;
using Common.Persistence.MSSQL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.MSSQL
{
    public sealed class SqlDbContext : DbContext, ISqlDbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
            Database.EnsureCreated();
            Database.Migrate();
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
        }
    }
}