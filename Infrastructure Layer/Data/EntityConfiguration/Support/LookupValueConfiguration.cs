using IMHub.Domain.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Data.EntityConfiguration.Support
{
    public class LookupValueConfiguration : IEntityTypeConfiguration<LookupValue>
    {
        public void Configure(EntityTypeBuilder<LookupValue> builder)
        {
            builder.Property(l => l.Category).HasMaxLength(50).IsRequired();
            builder.Property(l => l.Value).HasMaxLength(100).IsRequired();
            
            // Speed up searching by Category
            builder.HasIndex(l => l.Category);
        }
    }
}

