using System;
using System.Threading.Tasks;

namespace Common.Persistence.Postgres
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<Task> action);
    }
}