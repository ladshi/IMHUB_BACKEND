using IMHub.Domain.Entities;
using IMHub.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMHub.Infrastructure.Data.EntityConfiguration
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);
            builder.Property(u => u.JobTitle).HasMaxLength(100);
            builder.Property(u => u.Country).HasMaxLength(100);
        }
    }
}

