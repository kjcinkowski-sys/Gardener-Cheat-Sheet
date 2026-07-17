using System.Net;
using System.Text.Json;
using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.External;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GardenerCheatSheet.Infrastructure.Trefle;

/// <summary>
/// <see cref="ITreflePlantProvider"/> backed by the Trefle REST API. Responses
/// are cached in memory to respect Trefle's rate limits and to keep the app
/// responsive if Trefle is briefly unavailable.
/// </summary>
public sealed class TrefleClient : ITreflePlantProvider
{
    private const int PageSize = 20;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly TrefleOptions _options;
    private readonly ILogger<TrefleClient> _logger;

    public TrefleClient(
        HttpClient http,
        IMemoryCache cache,
        IOptions<TrefleOptions> options,
        ILogger<TrefleClient> logger)
    {
        _http = http;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public Task<TrefleSearchResult> SearchAsync(string query, int page, CancellationToken ct = default)
    {
        var trimmed = (query ?? string.Empty).Trim();
        var cacheKey = $"trefle:search:{trimmed.ToLowerInvariant()}:{page}";
        return GetOrAddAsync(cacheKey, () => FetchSearchAsync(trimmed, page, ct));
    }

    public Task<TreflePlantData?> GetByIdAsync(int trefleId, CancellationToken ct = default)
    {
        var cacheKey = $"trefle:detail:{trefleId}";
        return GetOrAddAsync(cacheKey, () => FetchDetailAsync(trefleId, ct));
    }

    private async Task<TrefleSearchResult> FetchSearchAsync(string query, int page, CancellationToken ct)
    {
        // Empty query lists the catalogue; a query hits the search endpoint.
        var path = string.IsNullOrEmpty(query)
            ? $"plants?page={page}&{TokenParam()}"
            : $"plants/search?q={Uri.EscapeDataString(query)}&page={page}&{TokenParam()}";

        var response = await SendAsync<TrefleListResponse>(path, ct)
                       ?? new TrefleListResponse();

        var items = response.Data ?? new List<TrefleListItem>();
        var total = response.Meta?.Total;

        return new TrefleSearchResult
        {
            Plants = items.Select(MapListItem).ToList(),
            Page = page,
            TotalPages = total is int t ? (int)Math.Ceiling(t / (double)PageSize) : null
        };
    }

    private async Task<TreflePlantData?> FetchDetailAsync(int trefleId, CancellationToken ct)
    {
        var response = await SendAsync<TrefleDetailResponse>($"plants/{trefleId}?{TokenParam()}", ct, allowNotFound: true);
        var detail = response?.Data;
        return detail is null ? null : MapDetail(detail);
    }

    private static TreflePlantData MapListItem(TrefleListItem item) => new()
    {
        TrefleId = item.Id,
        ScientificName = item.ScientificName ?? item.CommonName ?? $"Plant {item.Id}",
        CommonName = item.CommonName,
        Family = item.Family ?? item.FamilyCommonName,
        ImageUrl = item.ImageUrl
    };

    private static TreflePlantData MapDetail(TrefleDetail detail)
    {
        var growth = detail.MainSpecies?.Growth;
        return new TreflePlantData
        {
            TrefleId = detail.Id,
            ScientificName = detail.ScientificName ?? detail.CommonName ?? $"Plant {detail.Id}",
            CommonName = detail.CommonName,
            Family = detail.FamilyCommonName,
            ImageUrl = detail.ImageUrl,
            Description = detail.MainSpecies?.Observations ?? detail.Observations,
            Light = growth?.Light,
            AtmosphericHumidity = growth?.AtmosphericHumidity,
            MinimumPrecipitationMm = growth?.MinimumPrecipitation?.Mm is double min ? (int)Math.Round(min) : null,
            MaximumPrecipitationMm = growth?.MaximumPrecipitation?.Mm is double max ? (int)Math.Round(max) : null,
            SoilTexture = growth?.SoilTexture is int st ? $"Texture level {st}/10" : null
        };
    }

    private string TokenParam() => $"token={Uri.EscapeDataString(_options.Token)}";

    private async Task<T?> SendAsync<T>(string relativeUrl, CancellationToken ct, bool allowNotFound = false)
        where T : class
    {
        try
        {
            using var response = await _http.GetAsync(relativeUrl, ct);

            if (allowNotFound && response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new TrefleApiException(
                    $"Trefle request '{relativeUrl}' failed with status {(int)response.StatusCode}.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, ct);
        }
        catch (Exception ex) when (ex is not TrefleApiException)
        {
            _logger.LogError(ex, "Error calling Trefle: {Url}", relativeUrl);
            throw new TrefleApiException($"Error calling Trefle: {relativeUrl}", ex);
        }
    }

    private async Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory)
    {
        if (_cache.TryGetValue(cacheKey, out T? cached) && cached is not null)
        {
            return cached;
        }

        var value = await factory();
        _cache.Set(cacheKey, value, TimeSpan.FromMinutes(_options.CacheMinutes));
        return value;
    }
}
