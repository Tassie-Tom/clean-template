namespace Api.SharedKernal;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}

