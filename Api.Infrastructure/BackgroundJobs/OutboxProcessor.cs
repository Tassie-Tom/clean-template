using Api.Infrastructure.Outbox;
using Api.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Api.Infrastructure.BackgroundJobs;

public sealed class OutboxProcessor
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _mediator;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        ApplicationDbContext dbContext,
        IPublisher mediator,
        ILogger<OutboxProcessor> logger)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUTC == null)
            .OrderBy(m => m.OccurredOnUTC)
            .Take(20) // Process in batches
            .ToListAsync(cancellationToken);

        foreach (var outboxMessage in messages)
        {
            try
            {
                _logger.LogInformation(
                    "Processing outbox message {MessageId} of type {MessageType}",
                    outboxMessage.Id,
                    outboxMessage.EventType);

                Type? eventType = Type.GetType(outboxMessage.EventType);
                if (eventType is null)
                {
                    _logger.LogWarning("Cannot find type {EventType}", outboxMessage.EventType);
                    continue;
                }

                object? domainEvent = JsonSerializer.Deserialize(
                    outboxMessage.EventData,
                    eventType);

                if (domainEvent is null)
                {
                    _logger.LogWarning("Cannot deserialize event {MessageId}", outboxMessage.Id);
                    continue;
                }

                // Dispatch via MediatR
                await _mediator.Publish(domainEvent, cancellationToken);

                // Mark as processed
                outboxMessage.ProcessedOnUTC = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                outboxMessage.Error = ex.ToString();
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Error processing outbox message {MessageId}",
                    outboxMessage.Id);
            }
        }
    }
}