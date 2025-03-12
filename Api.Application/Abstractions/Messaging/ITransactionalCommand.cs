using Api.SharedKernel;
using MediatR;

namespace Api.Application.Abstractions.Messaging;

public interface ITransactionalCommand : ICommand;

public interface ITransactionalCommand<TResponse> : IRequest<Result<TResponse>>, ITransactionalCommand;
