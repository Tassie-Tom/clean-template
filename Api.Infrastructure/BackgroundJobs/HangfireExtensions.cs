using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.SharedKernel;

namespace Api.Infrastructure.BackgroundJobs;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfire(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Hangfire services
        services.AddHangfire((sp, config) =>
        {
            // Get the connection string using the SecretProvider
            var secretProvider = sp.GetRequiredService<ISecretProvider>();
            var connectionString = secretProvider.GetSecretAsync("ConnectionStrings:Database").GetAwaiter().GetResult();

            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString));
        });

        // Add the Hangfire server
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 4;
            options.Queues = new[] { "default", "outbox" };
        });

        // Register the OutboxProcessor
        services.AddScoped<OutboxProcessor>();

        return services;
    }
}