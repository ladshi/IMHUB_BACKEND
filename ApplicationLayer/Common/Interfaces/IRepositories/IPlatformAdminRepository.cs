using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IPlatformAdminRepository : IGenericRepository<PlatformAdmin>
    {
        Task<PlatformAdmin?> GetByEmailOrNameAsync(string identifier, CancellationToken cancellationToken = default);
    }
}

