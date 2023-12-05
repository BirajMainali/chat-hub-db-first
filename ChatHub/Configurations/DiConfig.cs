using ChatHub.Migrations;
using ChatHub.Migrations.Reporter;
using ChatHub.Migrations.Services;
using ChatHub.Providers;
using ChatHub.Providers.Interfaces;

namespace ChatHub.Configurations
{
    public static class DiConfig
    {
        public static IServiceCollection UseWebConfigurations(this IServiceCollection services)
        {
            services.AddTransient<IDbConnectionProvider, DbConnectionProvider>();
            services.AddTransient<IMigrationReporter, MigrationReporter>();
            services.AddTransient<IDatabaseMigration, DatabaseMigration>();
            services.AddTransient<IMigrationLogService, MigrationLogService>();
            return services;
        }
    }
}