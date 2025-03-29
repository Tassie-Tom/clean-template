using Api.Infrastructure.Outbox;

namespace Api.Application.Abstractions.Outbox;

public interface IOutboxMessageRepository
{
    void Add(OutboxMessage message);
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
}
