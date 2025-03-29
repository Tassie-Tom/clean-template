using System.Data;
using System.Linq.Expressions;
using Api.Application.Abstractions.Data;
using Api.Domain.Users;
using Api.Infrastructure.Outbox;
using Api.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Api.Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventDispatcher? domainEventDispatcher = null)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);
        ApplySoftDeleteQueryFilter(modelBuilder);
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        return (await Database.BeginTransactionAsync()).GetDbTransaction();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Collect all entities with domain events before saving
            var entitiesWithEvents = ChangeTracker.Entries<Entity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count != 0)
                .ToArray();

            // Save the changes to the database
            var result = await base.SaveChangesAsync(cancellationToken);

            // After successful save, dispatch domain events within the same transaction
            if (_domainEventDispatcher != null && entitiesWithEvents.Length != 0)
            {
                await _domainEventDispatcher.DispatchAndClearEventsAsync(entitiesWithEvents, cancellationToken);
            }
            else
            {
                // If no dispatcher is available, just clear the events
                foreach (var entity in entitiesWithEvents)
                {
                    entity.ClearDomainEvents();
                }
            }

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // This should be handled with a proper concurrency exception
            throw new Exception("Concurrency exception occurred", ex);
        }
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        // Get all types that inherit from AuditableEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(AuditableEntity).IsAssignableFrom(e.ClrType)))
        {
            // Create the method Info for the SetQueryFilter method
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var propertyMethodInfo = typeof(EF).GetMethod(nameof(EF.Property))
                ?.MakeGenericMethod(typeof(bool));

            var isDeletedProperty = Expression.Call(
                propertyMethodInfo!,
                parameter,
                Expression.Constant("IsDeleted"));

            var notDeletedExpression = Expression.Not(isDeletedProperty);
            var lambda = Expression.Lambda(notDeletedExpression, parameter);

            // Apply the filter
            entityType.SetQueryFilter(lambda);
        }
    }
}