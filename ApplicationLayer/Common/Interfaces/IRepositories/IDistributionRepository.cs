using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IDistributionRepository : IGenericRepository<Distribution>
    {
        Task<Distribution?> GetByOrganizationAndPrinterAsync(int organizationId, int printerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Distribution>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Distribution>> GetByPrinterIdAsync(int printerId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int organizationId, int printerId, CancellationToken cancellationToken = default);
    }
}

