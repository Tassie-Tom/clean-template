using Api.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Api.Application.Abstractions.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private static readonly Action<ILogger, string, Exception?> _processingRequest =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(Handle)),
            "Processing request {RequestName}");

    private static readonly Action<ILogger, string, Exception?> _completedRequest =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(Handle)),
            "Completed request {RequestName}");

    private static readonly Action<ILogger, string, Exception?> _completedRequestWithError =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(3, nameof(Handle)),
            "Completed request {RequestName} with error");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        _processingRequest(logger, requestName, null);

        TResponse result = await next();

        if (result.IsSuccess)
        {
            _completedRequest(logger, requestName, null);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                _completedRequestWithError(logger, requestName, null);
            }
        }

        return result;
    }
}