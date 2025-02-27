using Api.SharedKernal;

namespace Api.Domain.Users;
public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
