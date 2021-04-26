using System;
using System.Threading.Tasks;

namespace Common.Persitence.Postgres
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<Task> action);
    }
}