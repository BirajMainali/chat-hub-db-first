namespace ChatHub.Migrations;

public interface IDatabaseMigration
{
    Task RunMigrations();
}