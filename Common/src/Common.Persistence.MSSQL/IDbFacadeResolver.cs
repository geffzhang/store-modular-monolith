using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Common.Persistence.MSSQL
{
    public interface IDbFacadeResolver
    {
        DatabaseFacade Database { get; }
    }
}