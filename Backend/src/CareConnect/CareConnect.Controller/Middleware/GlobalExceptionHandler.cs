using CareConnect.DTOs.Errors;
using CareConnect.DTOs.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace CareConnect.Controller.Middleware;

/// <summary>
/// Single place that turns exceptions into HTTP responses: FluentValidation failures become a
/// clean 400, NotFoundException becomes 404, ForbiddenException becomes 403,
/// BusinessRuleViolationException becomes 409, and everything else becomes a generic 500 — the
/// real exception (message, stack trace, connection strings, SQL text, ...) is logged, never
/// returned to the caller.
/// </summary>
public sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationException => HandleValidationException(validationException),
            NotFoundException notFoundException => HandleKnownException(notFoundException, StatusCodes.Status404NotFound),
            ForbiddenException forbiddenException => HandleKnownException(forbiddenException, StatusCodes.Status403Forbidden),
            BusinessRuleViolationException businessRuleException => HandleKnownException(businessRuleException, StatusCodes.Status409Conflict),
            _ => HandleUnexpectedException(exception, httpContext),
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private (int StatusCode, ApiResponse<object>) HandleValidationException(ValidationException exception)
    {
        var errors = exception.Errors
            .Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}")
            .ToList();

        LogValidationFailed(errors.Count);

        return (StatusCodes.Status400BadRequest, ApiResponse<object>.Fail("Validation failed.", errors));
    }

    private (int StatusCode, ApiResponse<object>) HandleKnownException(Exception exception, int statusCode)
    {
        LogKnownException(exception.GetType().Name, exception.Message);

        return (statusCode, ApiResponse<object>.Fail(exception.Message));
    }

    private (int StatusCode, ApiResponse<object>) HandleUnexpectedException(Exception exception, HttpContext httpContext)
    {
        LogUnhandledException(exception, httpContext.Request.Path.Value ?? string.Empty);

        return (StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail("An unexpected error occurred."));
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Request failed validation with {ErrorCount} error(s).")]
    private partial void LogValidationFailed(int errorCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request failed with {ExceptionType}: {Message}")]
    private partial void LogKnownException(string exceptionType, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception while processing {Path}")]
    private partial void LogUnhandledException(Exception exception, string path);
}
