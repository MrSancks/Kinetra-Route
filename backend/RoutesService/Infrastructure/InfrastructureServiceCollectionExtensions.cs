using Microsoft.Extensions.DependencyInjection;
using RoutesService.Application.Ports;
using RoutesService.Infrastructure.Providers;
using RoutesService.Infrastructure.Repositories;

namespace RoutesService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddRouteInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IRoutePlanRepository, InMemoryRoutePlanRepository>();
        services.AddSingleton<IRiderLocationRepository, InMemoryRiderLocationRepository>();
        services.AddSingleton<IRouteOptimizationProvider, StubRouteOptimizationProvider>();
        return services;
    }
}
