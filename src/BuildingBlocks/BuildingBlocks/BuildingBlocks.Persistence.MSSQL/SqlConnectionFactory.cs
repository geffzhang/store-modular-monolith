using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Persistence.MSSQL
{
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly MssqlOptions _options;
        private IDbConnection _connection;

        public SqlConnectionFactory(IOptions<MssqlOptions> options)
        {
            _options = options.Value;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open) _connection.Dispose();
        }

        public IDbConnection GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_options.ConnectionString);
                _connection.Open();
            }

            return _connection;
        }

        public IDbConnection CreateNewConnection()
        {
            var connection = new SqlConnection(_options.ConnectionString);
            connection.Open();

            return connection;
        }

        public string GetConnectionString()
        {
            return _options.ConnectionString;
        }
    }
}