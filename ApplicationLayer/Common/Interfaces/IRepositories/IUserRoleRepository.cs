using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task<IReadOnlyList<UserRole>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}

