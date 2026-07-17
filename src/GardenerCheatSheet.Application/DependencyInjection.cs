using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GardenerCheatSheet.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers application-layer services. Infrastructure supplies the
    /// implementations of the repository and provider interfaces they depend on.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IWateringScheduleCalculator, WateringScheduleCalculator>();
        services.AddScoped<IPlantCatalogService, PlantCatalogService>();
        services.AddScoped<IGardenService, GardenService>();
        return services;
    }
}
