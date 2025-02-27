using Api.Application.Abstractions.Messaging;

namespace Api.Application.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
