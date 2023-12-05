using ChatHub.Migrations.ValueObject;

namespace ChatHub.Migrations.Reporter;

public interface IMigrationReporter
{
    List<string> GetMigrationsThroughFs();
    Task<List<Migration>> GetMigrationHistory();
    Task<List<string>> GetPendingMigrationFromFileSystem();

    /// <summary>
    /// This returns raw sql script of pending migrations file
    /// </summary>
    /// <returns></returns>
    Task<string> GetRawQueries(string path);

    List<string> GetForceMigration();
}