using CareConnect.AppServices.Caregivers;
using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.AppServices.DependencyInjection;

/// <summary>Composition boundary for application/business services.</summary>
public static class AppServicesServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ICaregiverActivationService, CaregiverActivationService>();

        return services;
    }
}
