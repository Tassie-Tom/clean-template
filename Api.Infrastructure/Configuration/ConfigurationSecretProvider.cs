using Api.SharedKernel;
using Microsoft.Extensions.Configuration;

namespace Api.Infrastructure.Configuration;

internal sealed class ConfigurationSecretProvider(IConfiguration configuration) : ISecretProvider
{
    public Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(configuration.GetValue<string>(secretName) ?? string.Empty);
    }

    public Task<T?> GetSecretAsTypeAsync<T>(string secretName, CancellationToken cancellationToken = default)
        where T : class
    {
        return Task.FromResult(configuration.GetSection(secretName).Get<T>());
    }
}