using Microsoft.Extensions.DependencyInjection;
using RoutesService.Application.Services;

namespace RoutesService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddRouteApplication(this IServiceCollection services)
    {
        services.AddScoped<IRoutePlanningService, RoutePlanningService>();
        services.AddScoped<IRiderTrackingService, RiderTrackingService>();
        services.AddScoped<IEtaService, EtaService>();
        return services;
    }
}
