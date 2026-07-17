namespace GardenerCheatSheet.Infrastructure.Trefle;

/// <summary>Raised when the Trefle API returns an unexpected/failed response.</summary>
public sealed class TrefleApiException : Exception
{
    public TrefleApiException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}
