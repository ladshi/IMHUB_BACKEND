using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ITemplateFieldRepository : IGenericRepository<TemplateField>
    {
        Task<IReadOnlyList<TemplateField>> GetByTemplatePageIdAsync(int templatePageId, CancellationToken cancellationToken = default);
        Task<TemplateField?> GetByTemplatePageIdAndFieldNameAsync(int templatePageId, string fieldName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TemplateField>> GetUnlockedFieldsByTemplatePageIdAsync(int templatePageId, CancellationToken cancellationToken = default);
    }
}

