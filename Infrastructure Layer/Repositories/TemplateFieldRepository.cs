using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class TemplateFieldRepository : GenericRepository<TemplateField>, ITemplateFieldRepository
    {
        public TemplateFieldRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<TemplateField>> GetByTemplatePageIdAsync(int templatePageId, CancellationToken cancellationToken = default)
        {
            return await _context.TemplateFields
                .Where(tf => tf.TemplatePageId == templatePageId)
                .ToListAsync(cancellationToken);
        }

        public async Task<TemplateField?> GetByTemplatePageIdAndFieldNameAsync(int templatePageId, string fieldName, CancellationToken cancellationToken = default)
        {
            return await _context.TemplateFields
                .FirstOrDefaultAsync(tf => tf.TemplatePageId == templatePageId && tf.FieldName == fieldName, cancellationToken);
        }

        public async Task<IReadOnlyList<TemplateField>> GetUnlockedFieldsByTemplatePageIdAsync(int templatePageId, CancellationToken cancellationToken = default)
        {
            return await _context.TemplateFields
                .Where(tf => tf.TemplatePageId == templatePageId && !tf.IsLocked)
                .ToListAsync(cancellationToken);
        }
    }
}

