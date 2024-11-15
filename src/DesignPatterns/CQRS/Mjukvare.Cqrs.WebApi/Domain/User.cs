namespace Mjukvare.Cqrs.WebApi.Domain;

public sealed class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public List<Checkin> Checkins { get; set; } = [];
}