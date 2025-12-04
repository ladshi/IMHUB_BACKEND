using IMHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Data.EntityConfiguration.Printers
{
    public class DistributionConfiguration : IEntityTypeConfiguration<Distribution>
    {
        public void Configure(EntityTypeBuilder<Distribution> builder)
        {
            // Usually just relationships, no special properties to config here yet.
        }
    }
}

