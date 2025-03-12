using System.Text.Json;
using Api.SharedKernel;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Api.Infrastructure.Configuration;

internal sealed class AzureKeyVaultSecretProvider : ISecretProvider
{
    private readonly SecretClient _secretClient;

    public AzureKeyVaultSecretProvider(string vaultUri)
    {
        // Use DefaultAzureCredential for managed identity or local dev authentication
        var credential = new DefaultAzureCredential();
        _secretClient = new SecretClient(new Uri(vaultUri), credential);
    }

    public async Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        try
        {
            KeyVaultSecret secret =
                await _secretClient.GetSecretAsync(secretName, cancellationToken: cancellationToken);
            return secret.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // Log secret not found
            return string.Empty;
        }
    }

    public async Task<T?> GetSecretAsTypeAsync<T>(string secretName, CancellationToken cancellationToken = default)
        where T : class
    {
        var secretValue = await GetSecretAsync(secretName, cancellationToken);
        return string.IsNullOrEmpty(secretValue) ? default : JsonSerializer.Deserialize<T>(secretValue);
    }
}