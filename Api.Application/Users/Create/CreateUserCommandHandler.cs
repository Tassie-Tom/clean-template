﻿using Api.Application.Abstractions.Data;
using Api.Application.Abstractions.Messaging;
using Api.Domain.Users;
using Api.SharedKernel;

namespace Api.Application.Users.Create;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        Email email = emailResult.Value;
        if (!await userRepository.IsEmailUniqueAsync(email))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        var name = new Name(command.Name);
        var user = User.Create(email, name, command.HasPublicProfile);

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
