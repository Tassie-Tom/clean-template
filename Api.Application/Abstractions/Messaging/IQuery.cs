using Api.SharedKernel;
using MediatR;

namespace Api.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
