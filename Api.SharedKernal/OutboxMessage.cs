namespace Api.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }

    public OutboxMessage(Guid id, DateTime occurredOnUTC, string eventType, string eventData)
    {
        Id = id;
        OccurredOnUTC = occurredOnUTC;
        EventType = eventType;
        EventData = eventData;
    }

    public DateTime OccurredOnUTC { get; init; }
    public string EventType { get; init; }
    public string EventData { get; init; }
    public DateTime? ProcessedOnUTC { get; set; }
    public string? Error { get; set; }
}
