using Api.Application.Abstractions.Authenication;
using Api.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Api.Infrastructure.Database.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;

    public AuditableEntityInterceptor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        string currentUser = _userContext.IsAuthenticated
                ? _userContext.UserId.ToString()
                : "System";
        
        var utcNow = DateTime.UtcNow;

        foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedBy(currentUser, utcNow);
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.SetModifiedBy(currentUser, utcNow);
                    break;
                    
                case EntityState.Deleted:
                    // If using soft delete pattern, change state to modified and set deleted properties
                    entry.State = EntityState.Modified;
                    entry.Entity.SetDeleted(currentUser, utcNow);
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
