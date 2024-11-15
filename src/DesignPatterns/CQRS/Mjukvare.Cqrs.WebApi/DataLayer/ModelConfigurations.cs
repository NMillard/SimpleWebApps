using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mjukvare.Cqrs.WebApi.Domain;

namespace Mjukvare.Cqrs.WebApi.DataLayer;

public sealed class CheckinConfiguration : IEntityTypeConfiguration<Checkin>
{
    public void Configure(EntityTypeBuilder<Checkin> builder)
    {
        builder.ToTable("Checkins");
    }
}
