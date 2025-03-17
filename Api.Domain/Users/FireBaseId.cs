using Api.SharedKernel;

namespace Api.Domain.Users;

public sealed record FireBaseId
{
    private FireBaseId(string value) => Value = value;
    public string Value { get; }
    public static Result<FireBaseId> Create(string? fireBaseId)
    {
        if (string.IsNullOrEmpty(fireBaseId))
        {
            return Result.Failure<FireBaseId>(FireBaseIdErrors.Empty);
        }
        return new FireBaseId(fireBaseId);
    }
}

internal static class FireBaseIdErrors
{
    public static readonly Error Empty = Error.Problem("FireBaseId.Empty", "FireBaseId is empty");
}