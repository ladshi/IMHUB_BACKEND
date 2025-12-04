using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class CsvUploadRepository : GenericRepository<CsvUpload>, ICsvUploadRepository
    {
        public CsvUploadRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<CsvUpload>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.CsvUploads
                .Where(c => c.OrganizationId == organizationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CsvUpload>> GetByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default)
        {
            return await _context.CsvUploads
                .Where(c => c.TemplateId == templateId)
                .ToListAsync(cancellationToken);
        }
    }
}

