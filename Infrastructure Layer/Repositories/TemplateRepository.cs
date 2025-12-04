using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class TemplateRepository : GenericRepository<Template>, ITemplateRepository
    {
        public TemplateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Template>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.Templates
                .Where(t => t.OrganizationId == organizationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Template?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Templates
                .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Templates
                .AnyAsync(t => t.Slug == slug, cancellationToken);
        }
    }
}

