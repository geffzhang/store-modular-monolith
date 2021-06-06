using System.Threading;
using System.Threading.Tasks;
using Common.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.MSSQL
{
    public interface ISqlDbContext
    {
        DbSet<OutboxMessage> OutboxMessages { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}