using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class ContentRepository : GenericRepository<Content>, IContentRepository
    {
        public ContentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Content>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.Contents
                .Where(c => c.OrganizationId == organizationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Content>> GetByTemplateVersionIdAsync(int templateVersionId, CancellationToken cancellationToken = default)
        {
            return await _context.Contents
                .Where(c => c.TemplateVersionId == templateVersionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Content>> GetByCsvUploadIdAsync(int csvUploadId, CancellationToken cancellationToken = default)
        {
            return await _context.Contents
                .Where(c => c.CsvUploadId == csvUploadId)
                .ToListAsync(cancellationToken);
        }
    }
}

