using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IContentFieldValueRepository : IGenericRepository<ContentFieldValue>
    {
        Task<IReadOnlyList<ContentFieldValue>> GetByContentIdAsync(int contentId, CancellationToken cancellationToken = default);
        Task<ContentFieldValue?> GetByContentIdAndTemplateFieldIdAsync(int contentId, int templateFieldId, CancellationToken cancellationToken = default);
    }
}

