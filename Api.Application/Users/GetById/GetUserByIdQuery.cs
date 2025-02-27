using Api.Application.Abstractions.Caching;

namespace Api.Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : ICachedQuery<UserResponse>
{
    public string CacheKey => $"user-by-id-{UserId}";

    public TimeSpan? Expiration => null;
}
