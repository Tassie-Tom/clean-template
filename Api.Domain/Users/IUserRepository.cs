﻿namespace Api.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsEmailUniqueAsync(Email email);

    void Insert(User user);

    Task<User?> GetByFirebaseIdAsync(string firebaseId);
}

