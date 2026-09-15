using System.Text.Json;
using CareConnect.Controller.Middleware;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Errors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace CareConnect.IntegrationTests;

/// <summary>
/// Exercises GlobalExceptionHandler directly (no HTTP server needed — IExceptionHandler.TryHandleAsync
/// takes a plain HttpContext) to verify every documented exception-to-status-code mapping and confirm
/// the response body always matches the project's ApiResponse envelope, and never leaks exception
/// detail for the unhandled-exception (500) path.
/// </summary>
public sealed class GlobalExceptionHandlerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly GlobalExceptionHandler _handler = new(NullLogger<GlobalExceptionHandler>.Instance);

    [Fact]
    public async Task ValidationException_MapsTo400_WithFieldLevelErrors()
    {
        var exception = new ValidationException(new[]
        {
            new ValidationFailure("StartTime", "StartTime must be before EndTime."),
            new ValidationFailure("CaregiverId", "CaregiverId is required."),
        });

        var (statusCode, body) = await InvokeAsync(exception);

        statusCode.Should().Be(StatusCodes.Status400BadRequest);
        body.Success.Should().BeFalse();
        body.Message.Should().Be("Validation failed.");
        body.Errors.Should().HaveCount(2);
        body.Errors.Should().Contain("StartTime: StartTime must be before EndTime.");
        body.Errors.Should().Contain("CaregiverId: CaregiverId is required.");
    }

    [Fact]
    public async Task NotFoundException_MapsTo404()
    {
        var exception = new NotFoundException("Caregiver 42 was not found.");

        var (statusCode, body) = await InvokeAsync(exception);

        statusCode.Should().Be(StatusCodes.Status404NotFound);
        body.Success.Should().BeFalse();
        body.Message.Should().Be("Caregiver 42 was not found.");
        body.Errors.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task ForbiddenException_MapsTo403()
    {
        var exception = new ForbiddenException("You may only view your own visits.");

        var (statusCode, body) = await InvokeAsync(exception);

        statusCode.Should().Be(StatusCodes.Status403Forbidden);
        body.Success.Should().BeFalse();
        body.Message.Should().Be("You may only view your own visits.");
    }

    [Fact]
    public async Task ConflictException_MapsTo409()
    {
        var exception = new ConflictException("Caregiver is already booked during this time.");

        var (statusCode, body) = await InvokeAsync(exception);

        statusCode.Should().Be(StatusCodes.Status409Conflict);
        body.Success.Should().BeFalse();
        body.Message.Should().Be("Caregiver is already booked during this time.");
    }

    [Fact]
    public async Task BusinessRuleException_MapsTo409()
    {
        var exception = new BusinessRuleException("Cannot check out: visit status is Scheduled, not InProgress.");

        var (statusCode, body) = await InvokeAsync(exception);

        statusCode.Should().Be(StatusCodes.Status409Conflict);
        body.Success.Should().BeFalse();
        body.Message.Should().Be("Cannot check out: visit status is Scheduled, not InProgress.");
    }

    [Fact]
    public async Task UnhandledException_MapsTo500_WithSafeMessageOnly()
    {
        var exception = new InvalidOperationException(
            "Connection string 'Server=prod-db;User Id=sa;Password=Sup3rSecret!' failed: SqlException at line 42.");

        var (statusCode, body) = await InvokeAsync(exception);

        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        body.Success.Should().BeFalse();
        body.Message.Should().Be("An unexpected error occurred. Please try again later.");
        body.Message.Should().NotContain("Sup3rSecret");
        body.Message.Should().NotContain("Connection string");
        body.Message.Should().NotContain("SqlException");
    }

    [Fact]
    public async Task EveryResponse_HasSuccessFalseAndConsistentShape()
    {
        Exception[] exceptions =
        [
            new NotFoundException("x"),
            new ForbiddenException("x"),
            new ConflictException("x"),
            new BusinessRuleException("x"),
            new InvalidOperationException("x"),
        ];

        foreach (var exception in exceptions)
        {
            var (_, body) = await InvokeAsync(exception);
            body.Success.Should().BeFalse();
            body.Message.Should().NotBeNullOrWhiteSpace();
        }
    }

    private async Task<(int StatusCode, ApiResponse<object> Body)> InvokeAsync(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);
        handled.Should().BeTrue();

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await JsonSerializer.DeserializeAsync<ApiResponse<object>>(httpContext.Response.Body, JsonOptions);

        return (httpContext.Response.StatusCode, body!);
    }
}
