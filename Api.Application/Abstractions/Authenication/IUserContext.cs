using Api.Domain.Users;

namespace Api.Application.Abstractions.Authenication;

public interface IUserContext
{
    Guid UserId { get; }
    string FirebaseId { get; }
    bool IsAuthenticated { get; }
    Task<User?> GetCurrentUserAsync();
}