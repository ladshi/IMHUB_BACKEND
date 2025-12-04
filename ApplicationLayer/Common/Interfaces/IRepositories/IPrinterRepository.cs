using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IPrinterRepository : IGenericRepository<Printer>
    {
        Task<Printer?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Printer>> GetByOrganizationIdAsync(int? organizationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Printer>> GetActivePrintersAsync(CancellationToken cancellationToken = default);
    }
}

