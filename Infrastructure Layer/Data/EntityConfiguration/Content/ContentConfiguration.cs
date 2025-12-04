using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Content
{

    public class ContentConfiguration : IEntityTypeConfiguration<IMHub.Domain.Entities.Content>
    {
        public void Configure(EntityTypeBuilder<IMHub.Domain.Entities.Content> builder)
        {
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Status).HasMaxLength(20).IsRequired();
        }
    }
}

