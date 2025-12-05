using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Templates
{
    public class TemplateFieldConfiguration : IEntityTypeConfiguration<TemplateField>
    {
        public void Configure(EntityTypeBuilder<TemplateField> builder)
        {
            // Decimal Precision (Important for Coordinates)
            // 10 digits total, 2 digits after decimal (e.g., 12345678.99)
            builder.Property(p => p.X).HasPrecision(10, 2);
            builder.Property(p => p.Y).HasPrecision(10, 2);
            builder.Property(p => p.Width).HasPrecision(10, 2);
            builder.Property(p => p.Height).HasPrecision(10, 2);

            // Enum Conversion (Optional: Store Enum as String if needed, but Int is standard)
            // builder.Property(p => p.FieldType).HasConversion<string>(); 
        }
    }
}

