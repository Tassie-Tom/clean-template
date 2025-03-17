using Api.Domain.Users;
using Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email)
    {
        return !await context.Users.AnyAsync(u => u.Email == email);
    }

    public void Insert(User user)
    {
        context.Users.Add(user);
    }

    public Task<User?> GetByFirebaseIdAsync(string firebaseId)
    {
        return context.Users.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId);
    }
}
