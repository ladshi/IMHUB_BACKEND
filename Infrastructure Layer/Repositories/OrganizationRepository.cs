using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class OrganizationRepository : GenericRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Organization?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(o => o.Name == name, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Organizations
                .AnyAsync(o => o.Name == name, cancellationToken);
        }
    }
}

