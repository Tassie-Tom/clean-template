using Api.Application.Abstractions.Caching;
using Api.Application.Abstractions.Data;
using Api.Domain.Users;
using Api.Infrastructure;
using Api.Infrastructure.Authentication;
using Api.Infrastructure.BackgroundJobs;
using Api.Infrastructure.Caching;
using Api.Infrastructure.Configuration;
using Api.Infrastructure.Database;
using Api.Infrastructure.Database.Interceptors;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Time;
using Api.SharedKernel;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddSecretProvider(configuration)
            .AddDatabase(configuration)
            .AddFirebaseAuthentication(configuration)
            .AddCaching(configuration)
            .AddHangfire(configuration) 
            .AddHealthChecks(configuration);

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());


        services.Configure<DbProviderOptions>(configuration.GetSection(DbProviderOptions.SectionName));

        var dbOptions = configuration.GetSection(DbProviderOptions.SectionName).Get<DbProviderOptions>()
                        ?? new DbProviderOptions();


        // Add database-specific connection factory
        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
            var secretProvider = sp.GetRequiredService<ISecretProvider>();
            var dbConnString = secretProvider.GetSecretAsync("ConnectionStrings:Database").GetAwaiter().GetResult();

            return dbOptions.Provider.ToLowerInvariant() switch
            {
                "postgres" => new PostgresConnectionFactory(dbConnString),
                "sqlserver" => new SqlServerConnectionFactory(dbConnString),
                "mysql" => new MySqlConnectionFactory(dbConnString),
                _ => throw new ArgumentException($"Unsupported database provider: {dbOptions.Provider}")
            };
        });

        services.AddScoped<AuditableEntityInterceptor>();


        // Configure DbContext
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var secretProvider = sp.GetRequiredService<ISecretProvider>();
            var dbConnString = secretProvider.GetSecretAsync("ConnectionStrings:Database").GetAwaiter().GetResult();

            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());


            options.ConfigureProvider(
                dbConnString,
                dbOptions.Provider,
                typeof(ApplicationDbContext).Assembly.GetName().Name!);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Cache")!;

        services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Cache")!);

        return services;
    }

    private static IServiceCollection AddSecretProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register options
        services.Configure<SecretProviderOptions>(
            configuration.GetSection(SecretProviderOptions.SectionName));

        // Register the appropriate secret provider based on configuration
        services.AddSingleton<ISecretProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SecretProviderOptions>>().Value;

            if (options.UseAzureKeyVault && !string.IsNullOrEmpty(options.KeyVaultUri))
            {
                return new AzureKeyVaultSecretProvider(options.KeyVaultUri);
            }

            return new ConfigurationSecretProvider(sp.GetRequiredService<IConfiguration>());
        });

        return services;
    }
}