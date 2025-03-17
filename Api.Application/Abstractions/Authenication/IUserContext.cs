using Api.Domain.Users;

namespace Api.Application.Abstractions.Authenication;

public interface IUserContext
{
    /// <summary>
    /// Gets the application's internal user ID
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets the Firebase user ID
    /// </summary>
    string FirebaseId { get; }

    /// <summary>
    /// Gets the current user entity if authenticated
    /// </summary>
    User? CurrentUser { get; }

    /// <summary>
    /// Whether the current request is from an authenticated user
    /// </summary>
    bool IsAuthenticated { get; }
}