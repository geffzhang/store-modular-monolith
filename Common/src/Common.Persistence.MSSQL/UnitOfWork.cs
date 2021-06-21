using System.Threading.Tasks;

namespace Common.Persistence.MSSQL
{
    public class UnitOfWork : IUnitOfWork 
    {
        private readonly ISqlDbContext _dbContext;

        public UnitOfWork(ISqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitTransactionAsync()
        {
            await _dbContext.CommitTransactionAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _dbContext.BeginTransactionAsync();
        }

        public async Task RollbackTransaction()
        {
            await _dbContext.RollbackTransaction();
        }
    }
}