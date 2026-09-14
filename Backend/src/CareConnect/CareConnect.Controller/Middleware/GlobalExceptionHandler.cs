using CareConnect.Infrastructure.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Middleware;

/// <summary>
/// Single place that turns exceptions into HTTP responses: FluentValidation failures become a
/// clean 400, NotFoundException becomes 404, ForbiddenException becomes 403,
/// BusinessRuleViolationException becomes 409, and everything else becomes a generic 500 — the
/// real exception (message, stack trace, connection strings, SQL text, ...) is logged, never
/// returned to the caller. All responses are written as RFC 7807 ProblemDetails via the
/// framework's IProblemDetailsService.
/// </summary>
public sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationException => HandleValidationException(validationException),
            NotFoundException notFoundException => HandleKnownException(notFoundException, StatusCodes.Status404NotFound, "Not Found"),
            ForbiddenException forbiddenException => HandleKnownException(forbiddenException, StatusCodes.Status403Forbidden, "Forbidden"),
            BusinessRuleViolationException businessRuleException => HandleKnownException(businessRuleException, StatusCodes.Status409Conflict, "Conflict"),
            _ => HandleUnexpectedException(exception, httpContext),
        };

        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });
    }

    private (int StatusCode, ProblemDetails ProblemDetails) HandleValidationException(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray());

        LogValidationFailed(exception.Errors.Count());

        return (StatusCodes.Status400BadRequest, new HttpValidationProblemDetails(errors)
        {
            Title = "Validation failed.",
            Status = StatusCodes.Status400BadRequest,
        });
    }

    private (int StatusCode, ProblemDetails ProblemDetails) HandleKnownException(Exception exception, int statusCode, string title)
    {
        LogKnownException(exception.GetType().Name, exception.Message);

        return (statusCode, new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = statusCode,
        });
    }

    private (int StatusCode, ProblemDetails ProblemDetails) HandleUnexpectedException(Exception exception, HttpContext httpContext)
    {
        LogUnhandledException(exception, httpContext.Request.Path.Value ?? string.Empty);

        return (StatusCodes.Status500InternalServerError, new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
        });
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Request failed validation with {ErrorCount} error(s).")]
    private partial void LogValidationFailed(int errorCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request failed with {ExceptionType}: {Message}")]
    private partial void LogKnownException(string exceptionType, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception while processing {Path}")]
    private partial void LogUnhandledException(Exception exception, string path);
}
