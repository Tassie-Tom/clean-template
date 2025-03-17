using Api.Application.Abstractions.Messaging;

namespace Api.Application.Users.Create;

public sealed record CreateUserCommand(string Email, string Name)
    : ICommand<Guid>;
