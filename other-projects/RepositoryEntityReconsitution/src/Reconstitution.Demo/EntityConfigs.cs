using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reconstitution.Demo;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Content);
        builder.Property(p => p.PublishDate).IsRequired(false);
        builder.HasOne<User>(p => p.Author).WithMany().HasForeignKey("UserId");
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();

        builder.HasMany(u => u.Claims)
            .WithMany("users")
            .UsingEntity<UserClaimInfrastructure>();
    }
}

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaimInfrastructure>
{
    public void Configure(EntityTypeBuilder<UserClaimInfrastructure> uc)
    {
        uc.ToTable("UserClaims");
        uc.Property(x => x.GrantedOn).IsRequired();

        uc.HasKey(x => new { x.UserId, x.ClaimId });
                
        uc.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).HasPrincipalKey(u => u.Id);
        uc.HasOne(x => x.Claim).WithMany().HasForeignKey(x => x.ClaimId).HasPrincipalKey(c => c.Id);
    }
}

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
    }
}

public class UserClaimInfrastructure
{
    public User User { get; set; }
    public Claim Claim { get; set; }
    public Guid UserId { get; set; }
    public Guid ClaimId { get; set; }
    public DateTimeOffset GrantedOn { get; set; }
}