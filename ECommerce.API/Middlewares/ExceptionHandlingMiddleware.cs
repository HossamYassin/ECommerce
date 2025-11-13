using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ECommerce.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred.");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.BadRequest,
                "One or more validation errors occurred.",
                ex.Errors.Select(error => error.ErrorMessage));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt.");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.Unauthorized,
                "Unauthorized access.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string detail,
        IEnumerable<string>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            status = context.Response.StatusCode,
            detail,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}

