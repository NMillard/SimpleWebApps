namespace Mjukvare.Cqrs.WebApi.Domain.ReadModels;

public sealed record UserCheckinDisplay
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required int TotalCheckins { get; init; }
    public required DateTimeOffset LatestCheckinDate { get; init; }
    public required List<CheckinDisplay> Checkins { get; init; } = [];
}