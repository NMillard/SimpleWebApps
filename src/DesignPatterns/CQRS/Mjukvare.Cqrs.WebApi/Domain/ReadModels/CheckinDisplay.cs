namespace Mjukvare.Cqrs.WebApi.Domain.ReadModels;

public sealed record CheckinDisplay
{
    public required Guid CheckinId { get; init; }
    public required string Text { get; init; }
    public required DateTimeOffset Created { get; init; }
}