using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class TemplateVersionRepository : GenericRepository<TemplateVersion>, ITemplateVersionRepository
    {
        public TemplateVersionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<TemplateVersion>> GetByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default)
        {
            return await _context.TemplateVersions
                .Where(tv => tv.TemplateId == templateId)
                .ToListAsync(cancellationToken);
        }

        public async Task<TemplateVersion?> GetActiveVersionByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default)
        {
            return await _context.TemplateVersions
                .FirstOrDefaultAsync(tv => tv.TemplateId == templateId && tv.IsActive, cancellationToken);
        }

        public async Task<TemplateVersion?> GetByTemplateIdAndVersionNumberAsync(int templateId, int versionNumber, CancellationToken cancellationToken = default)
        {
            return await _context.TemplateVersions
                .FirstOrDefaultAsync(tv => tv.TemplateId == templateId && tv.VersionNumber == versionNumber, cancellationToken);
        }
    }
}

