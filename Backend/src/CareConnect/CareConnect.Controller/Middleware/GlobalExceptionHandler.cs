using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CareConnect.Controller.Middleware;

/// <summary>
/// Single place that turns exceptions into HTTP responses, for every controller in the API.
/// FluentValidation failures become a clean 400, InvalidCredentialsException becomes 401,
/// NotFoundException becomes 404, ForbiddenException
/// becomes 403, ConflictException/BusinessRuleException become 409, and everything else becomes a
/// generic 500. In every case the response body is the project's standard envelope
/// (<see cref="ApiResponse{T}"/> with Data omitted) — never a stack trace, SQL text, connection
/// string, or raw exception detail. The real exception is always logged server-side first.
/// </summary>
public sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred. Please try again later.";

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
            InvalidCredentialsException invalidCredentialsException => HandleKnownException(invalidCredentialsException, StatusCodes.Status401Unauthorized),
            NotFoundException notFoundException => HandleKnownException(notFoundException, StatusCodes.Status404NotFound),
            ForbiddenException forbiddenException => HandleKnownException(forbiddenException, StatusCodes.Status403Forbidden),
            ConflictException conflictException => HandleKnownException(conflictException, StatusCodes.Status409Conflict),
            BusinessRuleException businessRuleException => HandleKnownException(businessRuleException, StatusCodes.Status409Conflict),
            _ => HandleUnexpectedException(exception, httpContext),
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private (int StatusCode, ApiResponse<object> Response) HandleValidationException(ValidationException exception)
    {
        var errors = exception.Errors
            .Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}")
            .ToArray();

        LogValidationFailed(errors.Length);

        return (StatusCodes.Status400BadRequest, ApiResponse<object>.Fail("Validation failed.", errors));
    }

    private (int StatusCode, ApiResponse<object> Response) HandleKnownException(Exception exception, int statusCode)
    {
        LogKnownException(exception.GetType().Name, exception.Message);

        return (statusCode, ApiResponse<object>.Fail(exception.Message));
    }

    private (int StatusCode, ApiResponse<object> Response) HandleUnexpectedException(Exception exception, HttpContext httpContext)
    {
        // Full exception (message, stack trace, and anything it wraps — e.g. raw SQL error text
        // or a connection string embedded in a SqlException) is logged here only. The caller only
        // ever sees UnexpectedErrorMessage below.
        LogUnhandledException(exception, httpContext.Request.Path.Value ?? string.Empty);

        return (StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(UnexpectedErrorMessage));
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Request failed validation with {ErrorCount} error(s).")]
    private partial void LogValidationFailed(int errorCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request failed with {ExceptionType}: {Message}")]
    private partial void LogKnownException(string exceptionType, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception while processing {Path}")]
    private partial void LogUnhandledException(Exception exception, string path);
}
