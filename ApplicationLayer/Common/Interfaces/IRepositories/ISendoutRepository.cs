using IMHub.Domain.Entities;
using IMHub.Domain.Enums;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ISendoutRepository : IGenericRepository<Sendout>
    {
        Task<IReadOnlyList<Sendout>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Sendout>> GetByContentIdAsync(int contentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Sendout>> GetByPrinterIdAsync(int printerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Sendout>> GetByStatusAsync(SendoutStatus status, CancellationToken cancellationToken = default);
        Task<Sendout?> GetByJobReferenceAsync(string jobReference, CancellationToken cancellationToken = default);
    }
}

