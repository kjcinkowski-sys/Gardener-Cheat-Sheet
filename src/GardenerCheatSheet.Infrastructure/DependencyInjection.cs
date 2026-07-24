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
        // Postgres in every environment. Prod supplies the connection string via the
        // ConnectionStrings__Default env var; the fallback matches the local docker-compose
        // Postgres so a bare `dotnet run` and design-time `dotnet ef` both work.
        var connectionString = configuration.GetConnectionString("Default")
                               ?? "Host=localhost;Port=5432;Database=gardener;Username=postgres;Password=postgres";

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

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
