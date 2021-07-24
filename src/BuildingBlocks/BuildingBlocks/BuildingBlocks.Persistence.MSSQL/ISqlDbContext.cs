using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.MSSQL
{
    public interface ISqlDbContext : IDbFacadeResolver, IDomainEventContext
    {
        DbSet<OutboxMessage> OutboxMessages { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollbackTransaction();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}