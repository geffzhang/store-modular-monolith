using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Persistence.MSSQL
{
    public interface IDbFacadeResolver
    {
        DatabaseFacade Database { get; }
    }
}