using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration.Identity
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            // Unique Index on Domain & TenantCode
            builder.HasIndex(o => o.Domain).IsUnique();
            builder.HasIndex(o => o.TenantCode).IsUnique();
        }
    }
}

