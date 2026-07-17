using GardenerCheatSheet.Application.External;

namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Reads plant data from the external Trefle API. Implemented in Infrastructure;
/// kept behind an interface so it can be cached, faked in tests, or swapped for
/// another provider (e.g. Perenual) without touching the application services.
/// </summary>
public interface ITreflePlantProvider
{
    Task<TrefleSearchResult> SearchAsync(string query, int page, CancellationToken ct = default);

    Task<TreflePlantData?> GetByIdAsync(int trefleId, CancellationToken ct = default);
}
