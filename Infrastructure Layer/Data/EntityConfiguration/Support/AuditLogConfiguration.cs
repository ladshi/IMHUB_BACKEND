using IMHub.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Support
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Action romba perusa irukkaathu. 50 characters pothum.
            builder.Property(x => x.Action).HasMaxLength(50).IsRequired();
            builder.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
            
            // JSON column: Unlimited size ok, but good to mark explicitly if needed
            // builder.Property(x => x.ChangesJson).HasColumnType("nvarchar(max)");
        }
    }
}

