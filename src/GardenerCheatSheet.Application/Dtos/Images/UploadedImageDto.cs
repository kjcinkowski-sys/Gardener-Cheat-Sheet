namespace GardenerCheatSheet.Application.Dtos;

/// <summary>The URL of a successfully stored upload.</summary>
public sealed record UploadedImageDto
{
    public required string Url { get; init; }
}
