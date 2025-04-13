namespace Api.SharedKernel;

/// <summary>
/// Defines operations for dispatching domain events
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a collection of domain events
    /// </summary>
    /// <param name="domainEvents">The collection of domain events to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches all domain events from an entity and clears its event collection
    /// </summary>
    /// <param name="entity">The entity containing domain events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DispatchAndClearEventsAsync(Entity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches all domain events from multiple entities and clears their event collections
    /// </summary>
    /// <param name="entities">The entities containing domain events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DispatchAndClearEventsAsync(IEnumerable<Entity> entities, CancellationToken cancellationToken = default);
}
