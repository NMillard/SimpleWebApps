using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polymorphism.WebApi.Users.Models;

namespace Polymorphism.WebApi.DataLayer.Configurations {
    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("Users");

            builder.Property(u => u.Id);
            builder.HasKey(nameof(User.Id));
            builder.Property(u => u.Username).HasMaxLength(50).IsRequired();

            builder.OwnsMany<Permission>(nameof(User.Permissions), permissionsBuilder => {
                permissionsBuilder.ToTable("Permissions");
                permissionsBuilder.Property(p => p.Type).HasMaxLength(150).IsRequired();
            });
        }
    }
}