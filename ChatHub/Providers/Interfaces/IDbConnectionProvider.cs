using Npgsql;

namespace ChatHub.Providers.Interfaces
{
    public interface IDbConnectionProvider
    {
        NpgsqlConnection CreateOpenConnection();
        NpgsqlConnection CreateServerAuthenticationConnection();
        string? GetDatabaseName();
    }
}
