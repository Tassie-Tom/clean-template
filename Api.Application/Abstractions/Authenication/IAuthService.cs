using Api.SharedKernel;

namespace Api.Application.Abstractions.Authenication;

public interface IAuthService
{
    Task<Result<UserIdentity>> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);

}

public record UserIdentity(Guid UserId, string Email, string Name);


public static class AuthErrors
{
    public static readonly Error InvalidToken = Error.Failure(
        "Auth.InvalidToken",
        "The provided token is invalid or expired");

    public static readonly Error TokenVerificationFailed = Error.Failure(
        "Auth.TokenVerificationFailed",
        "Token verification failed");

    public static readonly Error Unauthorized = Error.Failure(
        "Auth.Unauthorized",
        "User is not authorized to perform this action");
}
