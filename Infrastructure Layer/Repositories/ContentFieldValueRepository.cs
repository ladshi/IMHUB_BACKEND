using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class ContentFieldValueRepository : GenericRepository<ContentFieldValue>, IContentFieldValueRepository
    {
        public ContentFieldValueRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<ContentFieldValue>> GetByContentIdAsync(int contentId, CancellationToken cancellationToken = default)
        {
            return await _context.ContentFieldValues
                .Where(cfv => cfv.ContentId == contentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ContentFieldValue?> GetByContentIdAndTemplateFieldIdAsync(int contentId, int templateFieldId, CancellationToken cancellationToken = default)
        {
            return await _context.ContentFieldValues
                .FirstOrDefaultAsync(cfv => cfv.ContentId == contentId && cfv.TemplateFieldId == templateFieldId, cancellationToken);
        }
    }
}

