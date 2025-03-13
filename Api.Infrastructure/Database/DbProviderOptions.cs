using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Infrastructure.Database;

public class DbProviderOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "Postgres"; // Postgres, SqlServer, MySql, etc.
}


public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureProvider(
        this DbContextOptionsBuilder options,
        string connectionString,
        string provider,
        string migrationsAssembly)
    {
        return provider.ToLowerInvariant() switch
        {
            "postgres" => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)
                        .MigrationsAssembly(migrationsAssembly))
                .UseSnakeCaseNamingConvention(),

            "sqlserver" => options
                .UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)
                        .MigrationsAssembly(migrationsAssembly)),

            "mysql" => options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
                    mysqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)
                        .MigrationsAssembly(migrationsAssembly))
                .UseSnakeCaseNamingConvention(),

            _ => throw new ArgumentException($"Unsupported database provider: {provider}")
        };
    }
}
