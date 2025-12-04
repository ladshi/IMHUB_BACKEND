using IMHub.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Support
{
    public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
    {
        public void Configure(EntityTypeBuilder<NotificationLog> builder)
        {
            builder.Property(n => n.RecipientEmail).HasMaxLength(150).IsRequired();
            builder.Property(n => n.Subject).HasMaxLength(200);
            builder.Property(n => n.Status).HasMaxLength(20);
        }
    }
}

