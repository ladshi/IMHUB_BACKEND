using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ITemplateVersionRepository : IGenericRepository<TemplateVersion>
    {
        Task<IReadOnlyList<TemplateVersion>> GetByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default);
        Task<TemplateVersion?> GetActiveVersionByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default);
        Task<TemplateVersion?> GetByTemplateIdAndVersionNumberAsync(int templateId, int versionNumber, CancellationToken cancellationToken = default);
    }
}

