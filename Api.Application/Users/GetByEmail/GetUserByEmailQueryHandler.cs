using Api.Application.Abstractions.Data;
using Api.Application.Abstractions.Messaging;
using Api.Domain.Users;
using Dapper;
using System.Data;
using Api.SharedKernel;

namespace Api.Application.Users.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(IDbConnectionFactory factory)
    : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT
                u.id AS Id,
                u.email AS Email,
                u.name AS Name,
                u.has_public_profile AS HasPublicProfile
            FROM users u
            WHERE u.id = @Email
            """;

        using IDbConnection connection = factory.GetOpenConnection();

        UserResponse? user = await connection.QueryFirstOrDefaultAsync<UserResponse>(
            sql,
            query);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }

        return user;
    }
}
