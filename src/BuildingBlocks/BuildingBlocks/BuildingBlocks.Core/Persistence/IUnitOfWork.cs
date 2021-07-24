using System.Threading.Tasks;

namespace BuildingBlocks.Core.Persistence
{
    public interface IUnitOfWork
    {
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollbackTransaction();
    }
}