using System.Data;
using Api.Application.Abstractions.Data;
using Npgsql;

namespace Api.Infrastructure.Database;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public IDbConnection GetOpenConnection()
    {
        NpgsqlConnection connection = dataSource.OpenConnection();

        return connection;
    }
}
