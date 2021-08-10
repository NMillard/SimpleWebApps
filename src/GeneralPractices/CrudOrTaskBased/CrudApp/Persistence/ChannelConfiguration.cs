using CrudApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrudApp.Persistence {
    public class ChannelConfiguration : IEntityTypeConfiguration<ChatChannel> {
        public void Configure(EntityTypeBuilder<ChatChannel> builder) {
            builder.ToTable("ChatChannels");
            
            builder.Property(c => c.Id);
            builder.Property(c => c.Name).HasMaxLength(100);
            builder.Navigation(c => c.ChatMessages).HasField("messages");

            builder.OwnsMany<ChatMessage>(nameof(ChatChannel.ChatMessages), cBuilder => {
                cBuilder.HasKey(c => c.Id);
                cBuilder.Property(c => c.Content).HasMaxLength(1000).IsRequired();
            });
        }
    }
}