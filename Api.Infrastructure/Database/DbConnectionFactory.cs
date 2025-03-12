using System.Data;
using Api.Application.Abstractions.Data;
using Api.SharedKernel;
using Npgsql;

namespace Api.Infrastructure.Database;

internal sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public DbConnectionFactory(ISecretProvider secretProvider)
    {
        var connectionString = secretProvider.GetSecretAsync("Database:ConnectionString").GetAwaiter().GetResult();
        _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
    }

    public IDbConnection GetOpenConnection()
    {
        var connection = _dataSource.OpenConnection();
        return connection;
    }
}
