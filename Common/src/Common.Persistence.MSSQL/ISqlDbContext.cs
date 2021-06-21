using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Common.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.MSSQL
{
    public interface ISqlDbContext : IDbFacadeResolver
    {
        DbSet<OutboxMessage> OutboxMessages { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollbackTransaction();
    }
}