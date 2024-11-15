namespace Mjukvare.Cqrs.WebApi.Domain;

public sealed class Checkin
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Created { get; set; }
}