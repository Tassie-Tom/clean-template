namespace Api.SharedKernel;

public interface ISecretProvider
{
    Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);
    Task<T?> GetSecretAsTypeAsync<T>(string secretName, CancellationToken cancellationToken = default) where T : class;
}