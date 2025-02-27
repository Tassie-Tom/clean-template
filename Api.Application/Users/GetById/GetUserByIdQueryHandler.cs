using System.Data;
using Api.Application.Abstractions.Data;
using Api.Application.Abstractions.Messaging;
using Api.Domain.Users;
using Api.SharedKernal;
using Dapper;

namespace Api.Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT
                u.id AS Id,
                u.email AS Email,
                u.name AS Name,
                u.has_public_profile AS HasPublicProfile
            FROM users u
            WHERE u.id = @UserId
            """;

        using IDbConnection connection = factory.GetOpenConnection();

        UserResponse? user = await connection.QueryFirstOrDefaultAsync<UserResponse>(
            sql,
            query);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
        }

        return user;
    }
}
