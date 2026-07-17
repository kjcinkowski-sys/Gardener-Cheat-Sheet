namespace GardenerCheatSheet.Application.Dtos;

public sealed record WateringScheduleDto
{
    public required string Category { get; init; }
    public required int DaysBetweenWatering { get; init; }
    public required string Source { get; init; }
}
