using IMHub.Domain.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Support
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
            builder.Property(t => t.ColorHex).HasMaxLength(10);
        }
    }
}

