using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ITemplatePageRepository : IGenericRepository<TemplatePage>
    {
        Task<IReadOnlyList<TemplatePage>> GetByTemplateVersionIdAsync(int templateVersionId, CancellationToken cancellationToken = default);
        Task<TemplatePage?> GetByTemplateVersionIdAndPageNumberAsync(int templateVersionId, int pageNumber, CancellationToken cancellationToken = default);
    }
}

