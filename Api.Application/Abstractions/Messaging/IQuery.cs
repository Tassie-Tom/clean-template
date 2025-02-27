using Api.SharedKernal;
using MediatR;

namespace Api.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
