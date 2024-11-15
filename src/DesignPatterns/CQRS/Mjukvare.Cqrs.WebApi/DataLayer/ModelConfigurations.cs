using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mjukvare.Cqrs.WebApi.Domain;
using Mjukvare.Cqrs.WebApi.Domain.ReadModels;

namespace Mjukvare.Cqrs.WebApi.DataLayer;

public sealed class CheckinConfiguration : IEntityTypeConfiguration<Checkin>
{
    public void Configure(EntityTypeBuilder<Checkin> builder)
    {
        builder.ToTable("Checkins");
    }
}

public sealed class UserCheckinDisplayConfiguration : IEntityTypeConfiguration<UserCheckinDisplay>
{
    public void Configure(EntityTypeBuilder<UserCheckinDisplay> builder)
    {
        builder.ToTable("MaterializedUserCheckins");
        builder.HasKey(ucd => ucd.UserId);
        builder.Property(u => u.Checkins)
            .HasConversion<CheckinDisplayJsonConverter>()
            .HasColumnType("jsonb");
    }

    public class CheckinDisplayJsonConverter() : ValueConverter<List<CheckinDisplay>, string>(
        v => JsonSerializer.Serialize(v, Options),
        v => ConvertFrom(v))
    {
        private static List<CheckinDisplay> ConvertFrom(string value)
        {
            return JsonSerializer.Deserialize<List<CheckinDisplay>>(value, Options) ?? [];
        }

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}