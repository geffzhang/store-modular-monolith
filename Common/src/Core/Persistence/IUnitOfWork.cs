using System.Threading.Tasks;

namespace Common.Core.Persistence
{
    public interface IUnitOfWork
    {
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollbackTransaction();
    }
}