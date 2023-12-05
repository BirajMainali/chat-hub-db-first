using System.Data.Common;
using ChatHub.Providers.Interfaces;
using Dapper;
using Npgsql;

namespace ChatHub.Providers
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly IConfiguration _configuration;

        public DbConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NpgsqlConnection CreateOpenConnection()
        {
            var connectionString = GetConnectionString();
            return new NpgsqlConnection(connectionString);
        }

        public NpgsqlConnection CreateServerAuthenticationConnection()
        {
            var connectionString = GetConnectionString()?.Replace($"Database={GetDatabaseName()};", "");
            return new NpgsqlConnection(connectionString);
        }

        public string? GetDatabaseName()
        {
            var connectionString = GetConnectionString();
            var builder = new NpgsqlConnectionStringBuilder(connectionString: connectionString);
            return builder.Database;
        }

        private string? GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }
    }
}