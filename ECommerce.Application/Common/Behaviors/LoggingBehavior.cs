using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace ECommerce.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();

        // Log request details
        _logger.LogInformation(
            "Handling {RequestName} with RequestId {RequestId}. Request: {@Request}",
            requestName,
            requestGuid,
            request);

        try
        {
            // Process the request
            var response = await next();

            stopwatch.Stop();

            // Log successful completion with timing
            _logger.LogInformation(
                "Completed {RequestName} with RequestId {RequestId} in {ElapsedMilliseconds}ms. Response: {@Response}",
                requestName,
                requestGuid,
                stopwatch.ElapsedMilliseconds,
                response);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log error with full exception details and timing
            _logger.LogError(
                ex,
                "Error handling {RequestName} with RequestId {RequestId} after {ElapsedMilliseconds}ms. Request: {@Request}. Error: {ErrorMessage}",
                requestName,
                requestGuid,
                stopwatch.ElapsedMilliseconds,
                request,
                ex.Message);

            throw;
        }
    }
}

