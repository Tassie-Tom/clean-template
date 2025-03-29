using System.Text.Json;
using Api.Infrastructure.Outbox;
using Api.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.Infrastructure.DomainEvents;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly DbContext _dbContext;
    private readonly IPublisher _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        DbContext dbContext,
        IPublisher mediator,
        ILogger<DomainEventDispatcher> logger)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null || !domainEvents.Any())
        {
            return;
        }

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.GetType().AssemblyQualifiedName!,
            JsonSerializer.Serialize(domainEvent, domainEvent.GetType())))
            .ToList();

        await _dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            _logger.LogInformation("Domain event queued to outbox: {EventType}", domainEvent.GetType().Name);

            // Optionally publish immediately as well
            // await _mediator.Publish(domainEvent, cancellationToken);
        }
    }

    public async Task DispatchAndClearEventsAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        var events = entity.DomainEvents.ToList();
        entity.ClearDomainEvents();

        await DispatchEventsAsync(events, cancellationToken);
    }

    public async Task DispatchAndClearEventsAsync(IEnumerable<Entity> entities, CancellationToken cancellationToken = default)
    {
        var entitiesArray = entities as Entity[] ?? entities.ToArray();

        var allEvents = entitiesArray
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entity in entitiesArray)
        {
            entity.ClearDomainEvents();
        }

        await DispatchEventsAsync(allEvents, cancellationToken);
    }
}