using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Infrastructure.Database;
using Api.Infrastructure.Outbox;
using Api.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.Infrastructure.DomainEvents;

/// <summary>
/// Implements the domain event dispatcher pattern to handle domain events
/// through the outbox pattern for reliability.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DomainEventDispatcher> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public DomainEventDispatcher(
        ApplicationDbContext dbContext,
        IPublisher mediator,
        ILogger<DomainEventDispatcher> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Dispatches domain events to the outbox for reliable processing.
    /// </summary>
    /// <param name="domainEvents">Collection of domain events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DispatchEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        if (domainEvents is null || !domainEvents.Any())
        {
            return;
        }

        var eventsList = domainEvents.ToList();
        var timestamp = DateTime.UtcNow;

        try
        {
            // Create outbox messages for each domain event
            var outboxMessages = eventsList.Select(domainEvent =>
                CreateOutboxMessage(domainEvent, timestamp)).ToList();

            // Add messages to outbox
            await _dbContext.OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);

            // Log events that were queued
            foreach (var domainEvent in eventsList)
            {
                _logger.LogInformation("Domain event queued to outbox: {EventType}", domainEvent.GetType().Name);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while dispatching domain events");
            throw;
        }
    }

    /// <summary>
    /// Dispatches all domain events from an entity and clears them.
    /// </summary>
    /// <param name="entity">Entity with domain events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DispatchAndClearEventsAsync(
        Entity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var events = entity.DomainEvents.ToList();

        if (events.Count == 0)
        {
            return;
        }

        entity.ClearDomainEvents();
        await DispatchEventsAsync(events, cancellationToken);
    }

    /// <summary>
    /// Dispatches all domain events from multiple entities and clears them.
    /// </summary>
    /// <param name="entities">Collection of entities with domain events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task DispatchAndClearEventsAsync(
        IEnumerable<Entity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entitiesArray = entities.ToArray();

        if (entitiesArray.Length == 0)
        {
            return;
        }

        var allEvents = new List<IDomainEvent>();

        foreach (var entity in entitiesArray)
        {
            allEvents.AddRange(entity.DomainEvents);
            entity.ClearDomainEvents();
        }

        if (allEvents.Count != 0)
        {
            await DispatchEventsAsync(allEvents, cancellationToken);
        }
    }

    /// <summary>
    /// Creates an outbox message from a domain event.
    /// </summary>
    private static OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent, DateTime timestamp)
    {
        string eventTypeName = domainEvent.GetType().FullName!;
        string eventData;

        try
        {
            eventData = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _jsonOptions);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to serialize domain event of type {eventTypeName}", ex);
        }

        return new OutboxMessage(
            Guid.NewGuid(),
            timestamp,
            eventTypeName,
            eventData);
    }
}