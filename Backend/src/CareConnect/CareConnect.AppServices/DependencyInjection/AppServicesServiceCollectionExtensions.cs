using CareConnect.AppServices.Assignments;
using CareConnect.AppServices.Auth;
using CareConnect.AppServices.CaregiverAvailability;
using CareConnect.AppServices.Caregivers;
using CareConnect.AppServices.CareTasks;
using CareConnect.AppServices.Clients;
using CareConnect.AppServices.Dashboards;
using CareConnect.AppServices.Reporting;
using CareConnect.AppServices.Security;
using CareConnect.AppServices.Visits;
using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.AppServices.DependencyInjection;

/// <summary>Composition boundary for application/business services.</summary>
public static class AppServicesServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<ICareTasksAppService, CareTasksAppService>();
        services.AddScoped<ICaregiverAvailabilityAppService, CaregiverAvailabilityAppService>();
        services.AddScoped<IClientsAppService, ClientsAppService>();
        services.AddScoped<ICaregiversAppService, CaregiversAppService>();
        services.AddScoped<IAssignmentsAppService, AssignmentsAppService>();
        services.AddScoped<IVisitsAppService, VisitsAppService>();
        services.AddScoped<IVisitNotesAppService, VisitNotesAppService>();
        services.AddScoped<IReportingAppService, ReportingAppService>();
        services.AddScoped<IDashboardAppService, DashboardAppService>();

        return services;
    }
}
