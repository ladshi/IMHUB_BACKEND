using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Data.EntityConfiguration.Printers
{
    public class SendoutConfiguration : IEntityTypeConfiguration<Sendout>
    {
        public void Configure(EntityTypeBuilder<Sendout> builder)
        {
            builder.Property(s => s.JobReference).HasMaxLength(50).IsRequired();
            builder.Property(s => s.CurrentStatus).HasMaxLength(20);

            // Searching by JobReference will be fast
            builder.HasIndex(s => s.JobReference).IsUnique(); 
        }
    }
}

