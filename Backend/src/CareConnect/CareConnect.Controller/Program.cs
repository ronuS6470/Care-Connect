using CareConnect.AppServices.DependencyInjection;
using CareConnect.Commands;
using CareConnect.Commands.DependencyInjection;
using CareConnect.Controller.Middleware;
using CareConnect.Infrastructure.DependencyInjection;
using CareConnect.Queries;
using CareConnect.Queries.DependencyInjection;
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

// [ApiController]'s built-in InvalidModelStateResponseFactory already returns a
// ValidationProblemDetails for a malformed request body, matching the ProblemDetails shape
// GlobalExceptionHandler now uses for FluentValidation failures — no override needed.
builder.Services.AddControllers();

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
