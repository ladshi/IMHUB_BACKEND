using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Data.EntityConfiguration.Content
{
    public class ContentFieldValueConfiguration : IEntityTypeConfiguration<ContentFieldValue>
    {
        public void Configure(EntityTypeBuilder<ContentFieldValue> builder)
        {
            // Value can be large, but let's limit it if possible. 
            // For now, let's keep it default or max 4000.
            builder.Property(c => c.Value).HasMaxLength(4000);
        }
    }
}

