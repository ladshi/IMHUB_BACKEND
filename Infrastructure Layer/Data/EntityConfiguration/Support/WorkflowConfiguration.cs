using IMHub.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Support
{
    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.Property(w => w.Name).HasMaxLength(100).IsRequired();
            // StepsJson can be huge, so nvarchar(max) is default, which is fine.
        }
    }
}

