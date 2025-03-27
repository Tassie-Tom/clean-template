using System.Data;
using System.Linq.Expressions;
using System.Text.Json;
using Api.Application.Abstractions.Data;
using Api.Domain.Users;
using Api.Infrastructure.Outbox;
using Api.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Api.Infrastructure.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);

        //modelBuilder.Entity<AuditableEntity>().HasQueryFilter(e => !e.IsDeleted);

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
            AddDomainEventsAsOutboxMessages();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // todo 
            throw new Exception("Concurrency exception", ex);
            //throw new ConcurrencyException();
        }
 
    }

    public void AddDomainEventsAsOutboxMessages()
    {
        // Collect domain events from tracked entities
        var outboxMessages = ChangeTracker.Entries<Entity>().Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents;
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    domainEvent.GetType().Name,
                    JsonSerializer.Serialize(domainEvent)))
            .ToList();

        AddRange(outboxMessages);
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
