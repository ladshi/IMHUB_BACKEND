using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Data.EntityConfiguration.Printers
{
    public class SendoutStatusHistoryConfiguration : IEntityTypeConfiguration<SendoutStatusHistory>
    {
        public void Configure(EntityTypeBuilder<SendoutStatusHistory> builder)
        {
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);
            builder.Property(s => s.Notes).HasMaxLength(500);
        }
    }
}

