using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class DistributionRepository : GenericRepository<Distribution>, IDistributionRepository
    {
        public DistributionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Distribution?> GetByOrganizationAndPrinterAsync(int organizationId, int printerId, CancellationToken cancellationToken = default)
        {
            return await _context.Distributions
                .FirstOrDefaultAsync(d => d.OrganizationId == organizationId && d.PrinterId == printerId, cancellationToken);
        }

        public async Task<IReadOnlyList<Distribution>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.Distributions
                .Where(d => d.OrganizationId == organizationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Distribution>> GetByPrinterIdAsync(int printerId, CancellationToken cancellationToken = default)
        {
            return await _context.Distributions
                .Where(d => d.PrinterId == printerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int organizationId, int printerId, CancellationToken cancellationToken = default)
        {
            return await _context.Distributions
                .AnyAsync(d => d.OrganizationId == organizationId && d.PrinterId == printerId, cancellationToken);
        }
    }
}

