using System.Data;
using Api.Application.Abstractions.Data;
using Api.SharedKernel;
using Npgsql;

namespace Api.Infrastructure.Database;

internal sealed class PostgresConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();

    public IDbConnection GetOpenConnection()
    {
        var connection = _dataSource.OpenConnection();
        return connection;
    }
}


internal sealed class SqlServerConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection GetOpenConnection()
    {
        var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}


internal sealed class MySqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection GetOpenConnection()
    {
        var connection = new MySqlConnector.MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
