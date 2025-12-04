using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Data.EntityConfiguration.Content
{
    public class CsvUploadConfiguration : IEntityTypeConfiguration<CsvUpload>
    {
        public void Configure(EntityTypeBuilder<CsvUpload> builder)
        {
            builder.Property(c => c.FileName).HasMaxLength(255).IsRequired();
            builder.Property(c => c.FileUrl).IsRequired();
        }
    }
}

