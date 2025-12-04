using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IContentRepository : IGenericRepository<Content>
    {
        Task<IReadOnlyList<Content>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Content>> GetByTemplateVersionIdAsync(int templateVersionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Content>> GetByCsvUploadIdAsync(int csvUploadId, CancellationToken cancellationToken = default);
    }
}

