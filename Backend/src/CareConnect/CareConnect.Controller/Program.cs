using CareConnect.AppServices.DependencyInjection;
using CareConnect.Commands;
using CareConnect.Commands.DependencyInjection;
using CareConnect.Controller.Middleware;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.DependencyInjection;
using CareConnect.Queries;
using CareConnect.Queries.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment())
    .AddAppServices()
    .AddCommands()
    .AddQueries();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CommandsAssemblyMarker).Assembly,
    typeof(QueriesAssemblyMarker).Assembly));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// UseExceptionHandler() requires a fallback at startup (an ExceptionHandlingPath/ExceptionHandler,
// or a ProblemDetails writer) even though GlobalExceptionHandler unconditionally handles every
// exception itself and that fallback is therefore never reached. AddProblemDetails() only
// satisfies that requirement here — it does not change the response body, since our handler always
// runs first.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

// Model-binding failures (malformed body, [Required]/route-constraint mismatches) short-circuit
// before a handler — and therefore before GlobalExceptionHandler — ever runs. Overriding the
// factory keeps that 400 in the same ApiResponse envelope as every other validation failure
// instead of falling back to the framework's default ValidationProblemDetails shape.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => entry.Value!.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"))
            .ToArray();

        return new BadRequestObjectResult(ApiResponse<object>.Fail("Validation failed.", errors));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
