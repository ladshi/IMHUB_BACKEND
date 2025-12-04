using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ICsvUploadRepository : IGenericRepository<CsvUpload>
    {
        Task<IReadOnlyList<CsvUpload>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CsvUpload>> GetByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default);
    }
}

