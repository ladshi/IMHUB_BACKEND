using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Templates
{
    public class TemplateConfiguration : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            // Title is required and has max length
            builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
            
            // Slug for URL-friendly identifier
            builder.Property(t => t.Slug).HasMaxLength(250);
            
            // ThumbnailUrl can be long (blob storage URLs)
            builder.Property(t => t.ThumbnailUrl).HasMaxLength(1000);
            
            // Status enum conversion to string for better readability in database
            builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            
            // MetadataJson can be large, nvarchar(max) is default which is fine
            // builder.Property(t => t.MetadataJson).HasColumnType("nvarchar(max)");
            
            // Index on Slug for faster lookups
            builder.HasIndex(t => t.Slug);
            
            // Index on OrganizationId for faster queries
            builder.HasIndex(t => t.OrganizationId);
        }
    }
}

