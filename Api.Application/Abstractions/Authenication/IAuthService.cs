using Api.SharedKernel;

namespace Api.Application.Abstractions.Authenication;

public interface IAuthService
{
    Task<Result<UserIdentity>> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);

}

public record UserIdentity(Guid UserId, string Email, string Name);
