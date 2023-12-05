using ChatHub.Migrations.ValueObject;

namespace ChatHub.Migrations.Services;

public interface IMigrationLogService
{
    Task RecordMigration(Migration migration);
}