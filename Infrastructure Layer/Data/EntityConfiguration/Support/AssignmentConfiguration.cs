using IMHub.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Support
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            // Status length kuraikkanum
            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        }
    }
}

