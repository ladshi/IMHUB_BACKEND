using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Identity
{
    public class PlatformAdminConfiguration : IEntityTypeConfiguration<PlatformAdmin>
    {
        public void Configure(EntityTypeBuilder<PlatformAdmin> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Email).HasMaxLength(150).IsRequired();
            
            builder.HasIndex(p => p.Email).IsUnique(); // Unique Email
        }
    }
}

