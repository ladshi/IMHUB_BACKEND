using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Templates
{
    public class TemplatePageConfiguration : IEntityTypeConfiguration<TemplatePage>
    {
        public void Configure(EntityTypeBuilder<TemplatePage> builder)
        {
            builder.Property(p => p.Width).HasPrecision(10, 2);
            builder.Property(p => p.Height).HasPrecision(10, 2);
        }
    }
}

