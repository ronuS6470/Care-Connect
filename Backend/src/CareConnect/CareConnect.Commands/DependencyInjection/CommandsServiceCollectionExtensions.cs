using CareConnect.Commands.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.Commands.DependencyInjection;

/// <summary>Composition boundary for everything the Commands (write-side) layer owns.</summary>
public static class CommandsServiceCollectionExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CommandsAssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
