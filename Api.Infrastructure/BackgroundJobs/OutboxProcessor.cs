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
    private const int BatchSize = 20;

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
        _logger.LogInformation("Starting outbox message processing");

        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUTC == null && m.Error == null)
            .OrderBy(m => m.OccurredOnUTC)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            _logger.LogInformation("No outbox messages to process");
            return;
        }

        _logger.LogInformation("Processing {Count} outbox messages", messages.Count);

        foreach (var outboxMessage in messages)
        {
            await ProcessMessageAsync(outboxMessage, cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing outbox message {MessageId} of type {MessageType}",
                message.Id,
                message.EventType);

            Type? eventType = Type.GetType(message.EventType);
            if (eventType is null)
            {
                // Try with namespace lookup if we only stored the name
                eventType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == message.EventType || t.FullName == message.EventType);

                if (eventType is null)
                {
                    _logger.LogError("Cannot find type {EventType}", message.EventType);
                    await MarkMessageAsFailed(message, $"Cannot find type {message.EventType}", cancellationToken);
                    return;
                }
            }

            object? domainEvent = null;
            try
            {
                domainEvent = JsonSerializer.Deserialize(
                    message.EventData,
                    eventType);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Cannot deserialize event {MessageId}", message.Id);
                await MarkMessageAsFailed(message, $"Deserialization error: {ex.Message}", cancellationToken);
                return;
            }

            if (domainEvent is null)
            {
                _logger.LogError("Deserialized event is null for message {MessageId}", message.Id);
                await MarkMessageAsFailed(message, "Deserialized event is null", cancellationToken);
                return;
            }

            // Dispatch via MediatR
            try
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event {MessageId}", message.Id);
                await MarkMessageAsFailed(message, $"Publishing error: {ex.Message}", cancellationToken);
                return;
            }

            // Mark as processed
            message.ProcessedOnUTC = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully processed message {MessageId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error processing outbox message {MessageId}",
                message.Id);

            await MarkMessageAsFailed(message, $"Unexpected error: {ex.Message}", cancellationToken);
        }
    }

    private async Task MarkMessageAsFailed(
        OutboxMessage message,
        string error,
        CancellationToken cancellationToken)
    {
        try
        {
            message.Error = error;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to mark message {MessageId} as failed",
                message.Id);
        }
    }
}