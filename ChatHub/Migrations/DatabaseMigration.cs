using System.Transactions;
using ChatHub.Migrations.Reporter;
using ChatHub.Migrations.Services;
using ChatHub.Migrations.ValueObject;
using ChatHub.Providers.Interfaces;
using Dapper;

namespace ChatHub.Migrations;

public class DatabaseMigration : IDatabaseMigration
{
    private readonly IMigrationReporter _migrationReporter;
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly IMigrationLogService _migrationLogService;

    public DatabaseMigration(
        IMigrationReporter migrationReporter,
        IDbConnectionProvider dbConnectionProvider, IMigrationLogService migrationLogService)
    {
        _migrationReporter = migrationReporter;
        _dbConnectionProvider = dbConnectionProvider;
        _migrationLogService = migrationLogService;
    }

    public async Task RunMigrations()
    {
        var dbName = _dbConnectionProvider.GetDatabaseName();
        var databaseExist = await DatabaseExists(dbName);
        if (!databaseExist)
        {
            await CreateDatabase(dbName);
        }

        using var tsc = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        using var conn = _dbConnectionProvider.CreateOpenConnection();

        var migrationsRawQueries = _migrationReporter.GetForceMigration();
        foreach (var path in migrationsRawQueries)
        {
            var rawQuery = await _migrationReporter.GetRawQueries(path);
            await conn.ExecuteAsync(rawQuery);
            await _migrationLogService.RecordMigration(new Migration
            {
                Name = Path.GetFileName(path),
                Type = MigrationType.Applied,
                AppliedDate = DateTime.Now
            });
        }

        migrationsRawQueries = await _migrationReporter.GetPendingMigrationFromFileSystem();

        foreach (var path in migrationsRawQueries)
        {
            var rawQuery = await _migrationReporter.GetRawQueries(path);
            await conn.ExecuteAsync(rawQuery);
            await _migrationLogService.RecordMigration(new Migration
            {
                Name = Path.GetFileName(path),
                Type = MigrationType.Applied,
                AppliedDate = DateTime.Now
            });
        }

        tsc.Complete();
    }

    private async Task<bool> DatabaseExists(string dbName)
    {
        using var conn = _dbConnectionProvider.CreateServerAuthenticationConnection();
        const string query = @"select * from pg_database where datname = @dbName;";
        var result = await conn.ExecuteScalarAsync<int>(query, new { dbName = dbName });
        return result > 0;
    }

    private async Task CreateDatabase(string dbName)
    {
        using var conn = _dbConnectionProvider.CreateServerAuthenticationConnection();
        string createDbQuery = $"CREATE DATABASE {dbName}";
        await conn.ExecuteAsync(createDbQuery, new
        {
            dbName = dbName
        });
    }
}