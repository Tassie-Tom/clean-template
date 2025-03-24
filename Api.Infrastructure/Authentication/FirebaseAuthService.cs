using Api.Application.Abstractions.Authenication;
using Api.SharedKernel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace Api.Infrastructure.Authentication;

internal class FirebaseAuthService : IAuthService
{
    private readonly FirebaseAuth _firebaseAuth;
    private readonly ILogger<FirebaseAuthService> _logger;

    public FirebaseAuthService(FirebaseAuth firebaseAuth, ILogger<FirebaseAuthService> logger)
    {
        _firebaseAuth = firebaseAuth;
        _logger = logger;
    }
    public async Task<Result<UserIdentity>> VerifyTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(token, cancellationToken);

            var userIdentity = new UserIdentity(
                Guid.TryParse(decodedToken.Uid, out var userId) ? userId : Guid.Empty,
                decodedToken.Claims.TryGetValue("email", out object? email) ? email.ToString() : string.Empty,
                decodedToken.Claims.TryGetValue("name", out var name) ? name.ToString() : string.Empty);

            return userIdentity;
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Firebase token verification failed");
            return Result.Failure<UserIdentity>(AuthErrors.InvalidToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Firebase token");
            return Result.Failure<UserIdentity>(AuthErrors.TokenVerificationFailed);
        }
    }
}
