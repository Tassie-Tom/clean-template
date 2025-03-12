using Api.SharedKernel;

namespace Api.Domain.Users;
public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
