using ChatHub.Migrations.ValueObject;
using ChatHub.Providers.Interfaces;
using Dapper;

namespace ChatHub.Migrations.Reporter;

public class MigrationReporter : IMigrationReporter
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public MigrationReporter(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public List<string> GetMigrationsThroughFs()
    {
        var directoryPath = Path.Combine("Migrations", "RawQueries");
        var rawMigrations = Directory.GetFiles(directoryPath, "*.sql", SearchOption.AllDirectories);
        return rawMigrations.Select(file => file).ToList();
    }

    public async Task<List<Migration>> GetMigrationHistory()
    {
        const string query = @"select * from migrations";
        var conn = _dbConnectionProvider.CreateOpenConnection();
        return (await conn.QueryAsync<Migration>(query)).ToList();
    }

    public async Task<List<string>> GetPendingMigrationFromFileSystem()
    {
        var rawMigrationFiles = GetMigrationsThroughFs();
        var migrationsHistoryFromDb = await GetMigrationHistory();
        var appliedMigrationsFileNames = migrationsHistoryFromDb.Select(x => x.Name);
        return rawMigrationFiles.Where(x => !appliedMigrationsFileNames.Contains(Path.GetFileName(x))).ToList();
    }

    public async Task<string> GetRawQueries(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception($"File {filePath} does not exist");
        }

        var content = await ReadFileAsync(filePath);
        if (string.IsNullOrEmpty(content))
        {
            throw new InvalidOperationException($"{filePath} has not migrations script");
        }

        return content;
    }

    private async Task<string> ReadFileAsync(string filePath)
    {
        using var sr = new StreamReader(filePath);
        return await sr.ReadToEndAsync();
    }

    public List<string> GetForceMigration()
    {
        var args = new List<long>() { 1 };
        var migrations = GetMigrationsThroughFs();
        return migrations.Where(x => args.Contains(GetMigrationSequence(x))).ToList();
    }

    private long GetMigrationSequence(string name)
    {
        name = Path.GetFileName(name);
        var sequence = name.Split("__").FirstOrDefault();
        if (sequence == null)
        {
            throw new Exception("Migrations Sequence Not Found");
        }

        return Convert.ToInt64(sequence);
    }
}