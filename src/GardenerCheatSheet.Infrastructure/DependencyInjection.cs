using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Infrastructure.Persistence;
using GardenerCheatSheet.Infrastructure.Trefle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GardenerCheatSheet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
                               ?? "Data Source=gardener.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IPlantRepository, PlantRepository>();
        services.AddScoped<IGardenRepository, GardenRepository>();

        services.AddMemoryCache();
        services.Configure<TrefleOptions>(configuration.GetSection(TrefleOptions.SectionName));

        services.AddHttpClient<ITreflePlantProvider, TrefleClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<TrefleOptions>>().Value;
            var baseUrl = options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/";
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
