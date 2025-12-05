using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Templates
{
    public class TemplateVersionConfiguration : IEntityTypeConfiguration<TemplateVersion>
    {
        public void Configure(EntityTypeBuilder<TemplateVersion> builder)
        {
            // PdfUrl is required and can be long (blob storage URLs)
            builder.Property(tv => tv.PdfUrl).HasMaxLength(1000).IsRequired();
            
            // DesignJson can be large, nvarchar(max) is default which is fine
            // builder.Property(tv => tv.DesignJson).HasColumnType("nvarchar(max)");
            
            // VersionNumber should be positive
            builder.Property(tv => tv.VersionNumber).IsRequired();
            
            // Index on TemplateId for faster queries
            builder.HasIndex(tv => tv.TemplateId);
            
            // Composite index on TemplateId and VersionNumber for unique version lookup
            builder.HasIndex(tv => new { tv.TemplateId, tv.VersionNumber });
        }
    }
}

