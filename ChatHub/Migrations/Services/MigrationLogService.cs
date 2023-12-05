using ChatHub.Migrations.ValueObject;
using ChatHub.Providers.Interfaces;
using Dapper;

namespace ChatHub.Migrations.Services;

public class MigrationLogService : IMigrationLogService
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public MigrationLogService(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public async Task RecordMigration(Migration migration)
    {
        var query = @"insert into public.migrations (name, type, applieddate) values (@name, @type, @appliedDate);";
        using var conn = _dbConnectionProvider.CreateOpenConnection();
        await conn.ExecuteAsync(query, migration);
    }
}