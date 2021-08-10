using System;
using System.Linq;
using CrudApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrudApp.Persistence {
    public class AuthorConfiguration : IEntityTypeConfiguration<Author> {
        public void Configure(EntityTypeBuilder<Author> builder) {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Email);
            builder.Property(a => a.PenNames)
                .HasConversion(
                    p => string.Join(",", p),
                    p => p.Split(',', StringSplitOptions.TrimEntries).ToList()
                    );
        }
    }
}