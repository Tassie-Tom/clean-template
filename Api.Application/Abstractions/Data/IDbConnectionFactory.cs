using System.Data;

namespace Api.Application.Abstractions.Data;

public interface IDbConnectionFactory
{
    IDbConnection GetOpenConnection();
}
