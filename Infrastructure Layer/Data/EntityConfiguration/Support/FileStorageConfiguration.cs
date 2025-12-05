using IMHub.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Support
{
    public class FileStorageConfiguration : IEntityTypeConfiguration<FileStorage>
    {
        public void Configure(EntityTypeBuilder<FileStorage> builder)
        {
            builder.Property(f => f.FileName).HasMaxLength(255).IsRequired();
            builder.Property(f => f.FileType).HasMaxLength(50);
            builder.Property(f => f.FileUrl).IsRequired();
        }
    }
}

