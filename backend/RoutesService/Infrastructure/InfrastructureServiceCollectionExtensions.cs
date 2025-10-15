using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoutesService.Application.Ports;
using RoutesService.Infrastructure.Data;
using RoutesService.Infrastructure.Providers;
using RoutesService.Infrastructure.Repositories;

namespace RoutesService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddRouteInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured for RoutesService.");

        services.AddDbContext<RoutesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IRoutePlanRepository, RoutePlanRepository>();
        services.AddScoped<IRiderLocationRepository, RiderLocationRepository>();
        services.AddSingleton<IRouteOptimizationProvider, StubRouteOptimizationProvider>();
        return services;
    }
}
