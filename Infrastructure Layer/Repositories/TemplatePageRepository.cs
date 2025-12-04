using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class TemplatePageRepository : GenericRepository<TemplatePage>, ITemplatePageRepository
    {
        public TemplatePageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<TemplatePage>> GetByTemplateVersionIdAsync(int templateVersionId, CancellationToken cancellationToken = default)
        {
            return await _context.TemplatePages
                .Where(tp => tp.TemplateVersionId == templateVersionId)
                .OrderBy(tp => tp.PageNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<TemplatePage?> GetByTemplateVersionIdAndPageNumberAsync(int templateVersionId, int pageNumber, CancellationToken cancellationToken = default)
        {
            return await _context.TemplatePages
                .FirstOrDefaultAsync(tp => tp.TemplateVersionId == templateVersionId && tp.PageNumber == pageNumber, cancellationToken);
        }
    }
}

