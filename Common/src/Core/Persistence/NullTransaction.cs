using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Core.Persistence
{
    public class NullTransaction : ITransaction
    {
        private static readonly Lazy<NullTransaction> _instance = new ();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public static ITransaction Instance => _instance.Value;
    }
}