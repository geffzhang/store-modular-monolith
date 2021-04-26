using Common.Persistence;
using Dapper;
using OnlineStore.Modules.Users.Domain.UserRegistrations.DomainServices;

namespace OnlineStore.Modules.Users.Application.UserRegistrations
{
    //http://www.kamilgrzybek.com/design/domain-model-validation/
    public class UsersCounter : IUsersCounter
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UsersCounter(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public int CountUsersWithLogin(string login)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT " +
                               "COUNT(*) " +
                               "FROM [users].[v_Users] AS [User]" +
                               "WHERE [User].[Login] = @Login";
            return connection.QuerySingle<int>(
                sql,
                new
                {
                    login
                });
        }
    }
}