namespace Api.Infrastructure.Configuration;

public class SecretProviderOptions
{
    public const string SectionName = "SecretProvider";

    public bool UseAzureKeyVault { get; set; } = false;
    public string? KeyVaultUri { get; set; }
}