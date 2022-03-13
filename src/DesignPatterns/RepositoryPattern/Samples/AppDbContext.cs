using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RepositoryPattern.Samples {
    internal class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
    
    internal class UserConfig : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasMany(nameof(User.Books)).WithOne();
        }
    }

    internal class BookConfig : IEntityTypeConfiguration<Book> {
        public void Configure(EntityTypeBuilder<Book> builder) {
            builder.ToTable("Books");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title).HasMaxLength(100).IsRequired();
        }
    }
}