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
    .AddInfrastructure(builder.Configuration)
    .AddAppServices()
    .AddCommands()
    .AddQueries();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CommandsAssemblyMarker).Assembly,
    typeof(QueriesAssemblyMarker).Assembly));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Keeps the shape of a "malformed request body" 400 identical to a FluentValidation 400
        // (see GlobalExceptionHandler) instead of ASP.NET Core's default ValidationProblemDetails.
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"))
                .ToList();

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
