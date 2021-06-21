using System.Threading.Tasks;

namespace Common.Persistence
{
    public interface IUnitOfWork
    {
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollbackTransaction();
    }
}