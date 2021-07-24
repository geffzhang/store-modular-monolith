using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Types;
using BuildingBlocks.Core.Messaging.Outbox;
using BuildingBlocks.Persistence.MSSQL.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Persistence.MSSQL
{
    public abstract class SqlDbContextBase : DbContext, ISqlDbContext, IDomainEventContext
    {
        private IDbContextTransaction _currentTransaction;

        protected SqlDbContextBase(DbContextOptions<SqlDbContextBase> options) : base(options)
        {
            Database.EnsureCreated();
            DbUpInitializer.Initialize(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();

                await (_currentTransaction?.CommitAsync() ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransaction()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            var domainEntities = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x =>
                    x.Entity.Events != null &&
                    x.Entity.Events.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Events)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.Events?.ToList().Clear());

            return domainEvents;
        }
    }
}